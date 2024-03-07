using System.Reflection;
using API.Contract;
using API.Data;
using API.Data.SiteInitialisation;
using API.Extension;
using API.Jobs;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using ILogger = Microsoft.Extensions.Logging.ILogger;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    // .MinimumLevel.Override("Microsoft.AspNetCore",LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),new MSSqlServerSinkOptions()
    {
        TableName = "Serilog"
    })
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // options.SwaggerDoc("v4",new OpenApiInfo()
    // {
    //     Version = "v4",
    //     Title = "Taggloo API",
    //     Description = "API for autonomous interaction with Taggloo data",
    //     // TODO: set details
    //     
    // });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// this can't happen before migrations create the DB!
builder.Services.AddHangfire(configuration=>
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    // do not create tables for Hangfire, these are created as part of EF migrations
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions()
    {
        PrepareSchemaIfNecessary = false,
        TryAutoDetectSchemaDependentOptions = false
    }));
builder.Services.AddHangfireServer(options =>
{
    options.Queues = new string[] { "import" };
});

// builder.Services.AddHttpLogging(o => { });
// builder.Services.AddHttpLoggingInterceptor<HttpLoggingInterceptor>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(config =>
    {
        config.RouteTemplate = "/api/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(config =>
    {
        config.RoutePrefix = "api";
        config.SwaggerEndpoint("/api/v1/swagger.json","Taggloo API v4");
    });
}

#if !DEBUG
app.UseHttpsRedirection();
#endif

app.UseAuthorization();

//app.UseCors(builder => builder.AllowANyHeader().AllowAnyMethod()).WithOrigins("http://localhost:123"));

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHangfireDashboard("/admin/hangfire");

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        var dataContext = services.GetRequiredService<DataContext>();
        await dataContext.Database.MigrateAsync();
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        
        Initialiser initialiser = new Initialiser(userManager, roleManager, app.Environment.ContentRootPath, dataContext);
        SiteReadiness siteReadiness = await initialiser.GetSiteStatus();
        if (siteReadiness != SiteReadiness.Ready)
        {
            Log.Warning($"SiteReadiness requires initialisation: {siteReadiness}",new
            {
                siteReadiness
            });
            await initialiser.Initialise();    
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex,"An error occurred during migration");
        throw;
    }
}

IBackgroundJobClient backgroundJobClient = app.Services.GetRequiredService<IBackgroundJobClient>();
backgroundJobClient.Enqueue(() => Console.WriteLine("Hello from Hangfire"));

string? enforceDatabaseSizeCron = app.Configuration["Database:SizeManagement:CheckEvery"];
if (string.IsNullOrWhiteSpace(enforceDatabaseSizeCron))
    throw new NullReferenceException("Configuration Database:SizeManagement:CheckEvery must be specified");
RecurringJob.AddOrUpdate<EnforceDatabaseSizeJob>("databaseSize",job => job.EnforceDatabaseSize(), enforceDatabaseSizeCron);

app.Run();
