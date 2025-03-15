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
// defines services collection and adds DatabaseContext to it so every service like user, auth etc. will use
// the same database connection.

ILogger logger = new Logger("log.txt");

var services = new ServiceCollection();
services.AddTransient<ReservationRepository>();
services.AddDbContext<DatabaseContext>(); // could be AddSingleton too
services.AddSingleton(logger);
services.AddSingleton<AuthService>();
services.AddSingleton<IRbacService, RbacService>();
services.AddTransient<IUserRepository, UserRepository>();
services.AddTransient<UserService>();
// add more services here like this: services.AddTransient<MyService1>();

var serviceProvider = services.BuildServiceProvider();

// get services like this: var service1 = serviceProvider.GetService<MyService1>(); where MyService1 is a class that you added to
// services collection and in the class constructor you can inject other services like UserRepository, DatabaseContext etc.
var userService = serviceProvider.GetService<UserService>();
var authService = serviceProvider.GetService<AuthService>();
var reservationRepository = serviceProvider.GetService<ReservationRepository>();
if(authService == null || userService == null)
{
    throw new Exception("Services not found");
}

// under there will be defined menus and their options

var authMenu = new AuthMenu(authService);
var userMenu = new UserMenu(userService,authService,reservationRepository);
var adminMenu = new AdminMenu(userService);

bool showMenu = true;
while (showMenu)
{
    showMenu = authMenu.ShowMenu();
    var role = authService.GetLoggedUserRole();
    switch (role)
    {
        case Role.Admin:
            adminMenu.ShowAdminMenu();
            break;
        case Role.Client:
            userMenu.ShowUserMenu();
            break;
        default:
            Console.WriteLine("Unknown role");
            return;
    }
}