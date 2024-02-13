using System.Reflection;
using API.Data;
using API.Extension;
using API.Middleware;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationService(builder.Configuration);
// temporarily disabling logigng middleware
//builder.Services.Add(new ServiceDescriptor(typeof(IApiLogRepository),typeof(ApiLogRepository),ServiceLifetime.Singleton)); 
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
builder.Services.AddHangfireServer();

var app = builder.Build();

//app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#if !DEBUG
app.UseHttpsRedirection();
#endif

app.UseAuthorization();

//app.UseCors(builder => builder.AllowANyHeader().AllowAnyMethod()).WithOrigins("http://localhost:123"));

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<HttpLoggingMiddleware>();
app.MapControllers();

app.UseHangfireDashboard();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        var dataContext = services.GetRequiredService<DataContext>();
        await dataContext.Database.MigrateAsync();
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        ILogger<Initialiser> initialiserLogger = services.GetRequiredService<ILogger<Initialiser>>();
        Initialiser initialiser = new Initialiser(userManager, roleManager, initialiserLogger, app.Environment.ContentRootPath, dataContext);
        SiteReadiness siteReadiness = await initialiser.GetSiteStatus();
        if (siteReadiness != SiteReadiness.Ready)
        {
            initialiserLogger.LogWarning("SiteReadiness requires initialisation",new
            {
                siteReadiness
            });
            await initialiser.Initialise();    
        }
    }
    catch (Exception ex)
    {
        ILogger<Program>? programLogger = services.GetService<ILogger<Program>>();
        if (programLogger != null) programLogger.LogError(ex, "An error occurred during migration");
        else throw;
    }
}


IBackgroundJobClient backgroundJobClient = app.Services.GetRequiredService<IBackgroundJobClient>();
backgroundJobClient.Enqueue(() => Console.WriteLine("Hello from Hangfire"));

app.Run();
