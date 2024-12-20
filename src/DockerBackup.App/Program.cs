using Docker.DotNet;
using DockerBackup.App.Initializers;
using DockerBackup.App.Jobs;
using DockerBackup.Business.Helpers;
using DockerBackup.Business.Interfaces.Services;
using DockerBackup.Business.Services;
using DockerBackup.Model;
using DockerBackup.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

string connectionString = "data source=database.sqlite";

// Add services to the container.
builder.Services.AddDbContext<BackupsDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IDockerClient, DockerClient>(r =>
{
    var configurations = r.GetRequiredService<IConfiguration>();
    string serverUrl = !string.IsNullOrWhiteSpace(configurations["DOCKERBACKUP_SERVER_URL"]) ? configurations["DOCKERBACKUP_SERVER_URL"] : "unix:///var/run/docker.sock";

    var clientConfig = new DockerClientConfiguration(new Uri(serverUrl));

    return clientConfig.CreateClient();
});
    
builder.Services.AddScoped<IContainersService, ContainersService>();
builder.Services.AddScoped<ISchedulesService, SchedulesService>();

builder.Services.AddIdentity<User, Role>(options => { }).AddEntityFrameworkStores<BackupsDbContext>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.RetryInterval = TimeSpan.FromSeconds(60);

        s.UseSQLite(connectionString);
        s.UseNewtonsoftJsonSerializer();
    });
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddTransient<DockerVolumeBackupJob>();

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
    //app.UseHsts();
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

            //Create scheduler tables
            QuartzDbInitializer.Initialize(context.Database);
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
