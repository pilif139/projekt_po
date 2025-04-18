using Microsoft.Extensions.DependencyInjection;
using projekt_po.Database;
using DotNetEnv;
using projekt_po.Menu;
using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Services;
using projekt_po.Utils;

// uses DotNetEnv package to load secret variables from .env files
Env.Load();

// logger is a singleton service that will be used by other services like AuthService, UserRepository etc.
ILogger logger = new Logger("log.txt");

// defines services collection and adds DatabaseContext to it so every service like user, auth etc. will use
// the same database connection.
var services = new ServiceCollection();
services.AddDbContext<DatabaseContext>(); // could be AddSingleton too
services.AddSingleton(logger);

// services and repositories
services.AddSingleton<IAuthService, AuthService>();
services.AddSingleton<IRbacService, RbacService>();
services.AddTransient<IUserRepository, UserRepository>();
services.AddTransient<UserService>();
services.AddTransient<ILaneRepository, LaneRepository>();
services.AddTransient<LaneService>();
services.AddTransient<IReservationRepository, ReservationRepository>();
services.AddTransient<ReservationService>();

// database utils
services.AddTransient<Seeder>();
services.AddTransient<Cleaner>();

//menus
services.AddTransient<AuthMenu>();
services.AddTransient<ClientMenu>();
services.AddTransient<AdminMenu>();
services.AddTransient<WorkerMenu>();
// add more services here like this: services.AddTransient<MyService1>();

var serviceProvider = services.BuildServiceProvider();

string? adminLogin = Environment.GetEnvironmentVariable("ADMIN_LOGIN");
string? adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
if (!string.IsNullOrEmpty(adminLogin) && !string.IsNullOrEmpty(adminPassword))
{
    var seeder = serviceProvider.GetService<Seeder>();
    seeder?.AddAdminUser(adminLogin, adminPassword);
}

//handling seeder
var commandLineArgs = Environment.GetCommandLineArgs();
if (commandLineArgs.Contains("seed"))
{
    var seeder = serviceProvider.GetService<Seeder>()!;
    seeder.Seed();
    return;
} 
if (commandLineArgs.Contains("clean"))
{
    var cleaner = serviceProvider.GetService<Cleaner>()!;
    if (commandLineArgs.Contains("--database"))
    {
        cleaner.CleanDatabase();
    }
    else if (commandLineArgs.Contains("--log"))
    {
        cleaner.CleanLogger();
    }
    else
    {
        cleaner.CleanDatabase();
        cleaner.CleanLogger();
    }

    return;
}

// get services like this: var service1 = serviceProvider.GetService<MyService1>(); where MyService1 is a class that you added to
// services collection and in the class constructor you can inject other services like UserRepository, DatabaseContext etc.
var authMenu = serviceProvider.GetService<AuthMenu>();
var clientMenu = serviceProvider.GetService<ClientMenu>();
var adminMenu = serviceProvider.GetService<AdminMenu>();
var workerMenu = serviceProvider.GetService<WorkerMenu>();
var authService = serviceProvider.GetService<IAuthService>();

if (authMenu == null || clientMenu == null || adminMenu == null || authService == null)
{
    throw new Exception("Services not found");
}

bool showMenu = true;
while (showMenu)
{
    Console.Clear();
    // auth menu is shown first and it returns true if user logs in successfully and false if user wants to exit the app
    showMenu = authMenu.Show();
    //after logging in show menu based on user role
    var role = authService.GetLoggedUserRole();
    switch (role)
    {
        case Role.Admin:
            adminMenu.Show();
            break;
        case Role.Client:
            clientMenu.Show();
            break;
        case Role.Worker:
            workerMenu.Show();
            break;
        default:
            return;
    }
}
