using MudBlazor.Services;
using Serilog;
using BlazorTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BlazorTemplate.Server;
using BlazorTemplate.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
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

    await DbInitializer.Seed(app.Services);

    app.Run();

    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}
