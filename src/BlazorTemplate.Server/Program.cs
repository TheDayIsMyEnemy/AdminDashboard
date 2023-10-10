using MudBlazor.Services;
using Serilog;
using BlazorTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BlazorTemplate.Server;
using BlazorTemplate.Infrastructure;
using Microsoft.AspNetCore.Identity;
using BlazorTemplate.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var connectionStr = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContextFactory<AppDbContext>(
    options =>
        options
            .UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr))
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors());

builder.Services.AddSecurity();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.ConfigureOptions(builder.Configuration);

var app = builder.Build();

app.Logger.LogInformation("App created...");
app.Logger.LogInformation("Seeding Database...");

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var appDbContext = scopedProvider.GetRequiredService<AppDbContext>();
        await AppDbContextSeed.Seed(appDbContext, app.Logger);

        var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<Role>>();
        var appIdentityDbContext = scopedProvider.GetRequiredService<AppIdentityDbContext>();
        await AppIdentityDbContextSeed.Seed(appIdentityDbContext, userManager, roleManager, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the Database");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub().RequireAuthorization();
app.MapFallbackToPage("/_Host");

app.Logger.LogInformation("App is starting up...");
app.Run();
