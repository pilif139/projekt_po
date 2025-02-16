using projekt_po.Services;
using Spectre.Console;

public class UserMenu
{
     private readonly UserService _userService;

     public UserMenu(UserService userService)
     {
         _userService = userService;
     }

     public void ShowUserMenu()
     {
         Console.Clear();
         while (true)
         {
             Console.Clear();
             var Option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                 .Title("[red]User menu[/]")
                 .PageSize(10)
                 .AddChoices(new[]
                 {
                     "Make reservation",
                     "Your reservations",
                     "Logout"
                 }));
             switch (Option)
             {
                 case "Make reservation":
                     MakeReservation();
                     break;
                 case "Your reservations":
                     ShowUserReservations();
                     break;
                 case "Logout":
                     Console.WriteLine("Logging out...");
                     return;
             }
         }
     }
     
     private void MakeReservation()
     {
         Console.Clear();
         Console.WriteLine("Making reservation...");
     }
     
     private void ShowUserReservations()
     {
         Console.Clear();
         Console.WriteLine("Showing user reservations...");
     }
}