using Serilog;
using MudBlazor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BlazorTemplate.Web.Components;
using BlazorTemplate.Web.Components.Account;
using BlazorTemplate.Infrastructure.Data;
using BlazorTemplate.Infrastructure.Identity;
using BlazorTemplate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var dbConnectionString = builder.Configuration.GetConnectionString("BlazorTemplate");
var identityDbConnectionString = builder.Configuration.GetConnectionString("BlazorTemplate.Identity");
builder.Services.AddDbContextFactory<AppDbContext>(
    options =>
        options
            .UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString))
            .LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddDbContextFactory<AppIdentityDbContext>(
    options =>
        options
            .UseMySql(identityDbConnectionString, ServerVersion.AutoDetect(identityDbConnectionString))
            .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services
    .AddIdentity<User, Role>(options =>
    {
        options.Stores.MaxLengthForKeys = 128;
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(24);
        options.Lockout.MaxFailedAccessAttempts = 3;
        // options.User.AllowedUserNameCharacters = null;
    })
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddUserConfirmation<UserConfirmation>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(5);
    options.SlidingExpiration = true;
    options.LoginPath = new PathString(Constants.LoginPath);
    options.LogoutPath = new PathString(Constants.LogoutPath);
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

var app = builder.Build();

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
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

app.Run();
