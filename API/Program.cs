using System.Reflection;
using System.Text;
using API.Contract;
using API.Data;
using API.Extension;
using API.Middleware;
using API.Model;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.SqlServer;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

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
    // xonfigure to not build tables
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions()
    {
        PrepareSchemaIfNecessary = false,
        TryAutoDetectSchemaDependentOptions = false
    }));
builder.Services.AddHangfireServer();

var app = builder.Build();


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
        // TODO: what if cannot connect to SQL Server?
        await dataContext.Database.MigrateAsync();
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        ILogger logger = services.GetRequiredService<ILogger>();
        Initialiser initialiser = new Initialiser(userManager, roleManager, logger, app.Environment.ContentRootPath, dataContext);
        SiteReadiness siteReadiness = await initialiser.GetSiteStatus();
        if (siteReadiness != SiteReadiness.Ready)
        {
            await initialiser.Initialise();    
        }
        
        
        
        //await Seed.SeedUsers(userManager,roleManager,app.Environment.ContentRootPath);
        
        
    }
    catch (Exception ex)
    {
        ILogger<Program>? logger = services.GetService<ILogger<Program>>();
        if (logger != null) logger.LogError(ex, "An error occurred during migration");
        else throw;
    }
}


IBackgroundJobClient backgroundJobClient = app.Services.GetRequiredService<IBackgroundJobClient>();
backgroundJobClient.Enqueue(() => Console.WriteLine("Hello from Hangfire"));

app.Run();
