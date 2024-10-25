using DockerBackup.Business.Helpers;
using DockerBackup.Model;
using DockerBackup.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BackupsDbContext>(options => options.UseSqlite("data source=database.sqlite"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, Role>(options => { }).AddEntityFrameworkStores<BackupsDbContext>();
builder.Services.AddControllersWithViews();

//Configure logger
builder.Host.UseSerilog();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
Log.Logger = logger;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Initialize database if required
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<BackupsDbContext>();
    var log = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    context.Database.Migrate();

    var anyUsers = context.Users.Any();

    if (!anyUsers)
    {
        //If there aren't any users, initialize with the default admin user
        var configurations = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
        bool wasPasswordSupplied = !string.IsNullOrWhiteSpace(configurations["DOCKERBACKUP_ADMIN_PASSWORD"]);

        string adminPassword = wasPasswordSupplied ? configurations["DOCKERBACKUP_ADMIN_PASSWORD"] : new PasswordGenerator(new PasswordGeneratorOptions(){ PasswordLength = 16, SymbolsAmount = 1}).Generate();

        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var userToCreate = new User() { UserName = "admin" };
        IdentityResult result = await userManager.CreateAsync(userToCreate, adminPassword);

        if (result.Succeeded)
        {
            log.LogInformation($"Admin user created { (wasPasswordSupplied ? "with environment variable value" : "with password:" + adminPassword) }");
        }
        else
        {
            log.LogError($"admin user initialization failed:{string.Join(",", result.Errors.Select(e => e.Description))}");

            var appLifetime = serviceScope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
            appLifetime.StopApplication();
        }
    }
}

app.Run();
