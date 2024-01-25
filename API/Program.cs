using System.Reflection;
using System.Text;
using API.Contract;
using API.Data;
using API.Extension;
using API.Model;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

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

app.MapControllers();


using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        var dataContext = services.GetRequiredService<DataContext>();
        await dataContext.Database.MigrateAsync();
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await Seed.SeedUsers(userManager,roleManager);
    }
    catch (Exception ex)
    {
        ILogger<Program>? logger = services.GetService<ILogger<Program>>();
        if (logger != null) logger.LogError(ex, "An error occurred during migration");
        else throw;
    }
}
app.Run();
