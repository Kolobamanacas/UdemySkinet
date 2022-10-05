using API.Extensionos;
using API.Helpers;
using API.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.IO;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLLocal")));
builder.Services.AddDbContext<AppIdentityDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>((localConfig) =>
{
    ConfigurationOptions config = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCors(
    option => option.AddPolicy(
        "CorsPolicy",
        policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")));

// Configure the HTTP requrest pipeline.
WebApplication app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
    RequestPath = "/content"
});

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerDocumentation();
app.MapControllers();
app.MapFallbackToController("Index", "Fallback");

using IServiceScope scope = app.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;
ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    StoreContext context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context, loggerFactory);

    UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
    AppIdentityDbContext identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception exception)
{
    ILogger logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(exception, "An error occure during migration");
}

await app.RunAsync();
