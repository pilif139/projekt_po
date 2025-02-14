﻿using Microsoft.Extensions.DependencyInjection;
using projekt_po.Database;
using DotNetEnv;
using projekt_po.Repository;
using projekt_po.Services;
using projekt_po.Utils;

// uses DotNetEnv package to load secret variables from .env files
Env.Load();

// defines services collection and adds DatabaseContext to it so every service like user, auth etc. will use
// the same database connection.

ILogger logger = new Logger("log.txt");

var services = new ServiceCollection();
services.AddDbContext<DatabaseContext>(); // could be AddSingleton too
services.AddSingleton(logger);
services.AddTransient<IUserRepository, UserRepository>();
services.AddTransient<UserService>();
// add more services here like this: services.AddTransient<MyService1>();

var serviceProvider = services.BuildServiceProvider();

// get services like this: var service1 = serviceProvider.GetService<MyService1>(); where MyService1 is a class that you added to
// services collection and in the class constructor you can inject other services like UserRepository, DatabaseContext etc.
var userService = serviceProvider.GetService<UserService>();
if (userService == null)
{
    throw new Exception("UserService not found in services collection.");
}

userService.ListUsers();
// under there will be defined menus and their options
