using Microsoft.Extensions.DependencyInjection;
using projekt_po.Database;
using DotNetEnv;

// uses DotNetEnv package to load secret variables from .env files
Env.Load();


// defines services collection and adds DatabaseContext Singleton to it so every service like user, auth etc. will use the same database connection.
var services = new ServiceCollection();
services.AddSingleton<DatabaseContext>();
// add more services here like this: services.AddTransient<MyService1>();

var serviceProvider = services.BuildServiceProvider();

// get services like this: var service1 = serviceProvider.GetService<MyService1>(); where MyService1 is a class that you added to services collection and in the class definiton you have readonly DatabaseContext _context field with DatabaseContext injected in the constructor.

System.Console.WriteLine("Hello World!");