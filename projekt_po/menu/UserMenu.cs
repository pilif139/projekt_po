using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Services;
using Spectre.Console;

public class UserMenu
{
     private readonly UserService _userService;
     private readonly AuthService _authService;
     private readonly ReservationRepository _reservationRepository;

     public UserMenu(UserService userService, AuthService authService, ReservationRepository reservationRepository)
     {
         _userService = userService;
         _authService = authService;
         _reservationRepository = reservationRepository;
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
         Console.Write("Enter details about reservation: ");
         string details = Console.ReadLine();
         Console.Write("Enter date of reservation (yyyy-mm-dd HH:mm): )");
         string ReservationDate = Console.ReadLine();
         while (!DateTime.TryParse(ReservationDate, out DateTime reservationDate))
         {
             Console.WriteLine("Error: Wrong date format!");
             Console.Write("Enter date of reservation (yyyy-mm-dd HH:mm): )");
             ReservationDate = Console.ReadLine();
         }
         DateTime date = DateTime.Parse(ReservationDate);
         var user = _authService.GetLoggedUser();
         var reservation = _reservationRepository.Add(user.Id,details,date);
     }
     
     private void ShowUserReservations()
     {
         Console.Clear();
         Console.WriteLine("Showing user reservations...");
     }
}