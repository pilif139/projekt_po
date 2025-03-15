using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Services;
using Spectre.Console;
using projekt_po.Database;

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
                     "Delete reservation",
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
                 case "Delete reservation":
                     DeleteReservation();
                     break;
                 case "Logout":
                     Console.WriteLine("Logging out...");
                     return;
             }
         }
     }

     private void DeleteReservation()
     {
         Console.Clear();
         int UserId=_authService.GetLoggedUser().Id;
         var reservations = _reservationRepository.GetReservations(UserId);
         if (reservations == null)
         {
             Console.WriteLine("No reservations to delete found.");
             Console.WriteLine("Press any key to continue...");
             Console.ReadKey();
             return;
         }
         var reservationsToDelete = AnsiConsole.Prompt(
             new MultiSelectionPrompt<Reservation>()
                 .Title("Choose reservations to delete")
                 .NotRequired()
                 .PageSize(15)
                 .MoreChoicesText("[grey](Move up and down))[/]")
                 .InstructionsText(
                     "[grey](Press [blue]<space>[/] to pick a reservation to delete, " + 
                     "[green]<enter>[/] to accept)[/]")
                 .AddChoices(reservations));
        
         foreach (var rez in reservationsToDelete)
         {
             _reservationRepository.Delete(rez.Id);
         }
         if(reservationsToDelete.Count > 0)
         {
             Console.WriteLine("Reservations deleted successfully.");
         }
         else
         {
             Console.WriteLine("No reservations deleted.");
         }

         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
     }
     
     private void MakeReservation()
{
    Console.Clear();
    Console.Write("Enter details about reservation: ");
    string details = Console.ReadLine();

   
    DateTime reservationDate = default;
    bool dateAvailable = false;
    
    while (!dateAvailable)
    {
        Console.Write("Enter date of reservation (yyyy-mm-dd HH:mm): ");
        string ReservationDate = Console.ReadLine();
        
        while (!DateTime.TryParse(ReservationDate, out reservationDate))
        {
            Console.WriteLine("Error: Wrong date format!");
            Console.Write("Enter date of reservation (yyyy-mm-dd HH:mm): ");
            ReservationDate = Console.ReadLine();
        }

        
        if (CheckIfIsAlreadyReserved(reservationDate))
        {
            continue;
        }

        dateAvailable = true;  
    }

    var user = _authService.GetLoggedUser();
    var reservation = _reservationRepository.Add(user.Id, details, reservationDate);
}

public bool CheckIfIsAlreadyReserved(DateTime date)
{
    using (var context = new DatabaseContext())
    {
       
        int numberOfReservations = context.Reservations.Count(r => r.Date == date);

        if (numberOfReservations > 0)
        {
          
            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[red]Sorry. This time is already occupied![/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Change date of reservation",
                    "Cancel reservation"
                }));

            switch (option)
            {
                case "Change date of reservation":
                   
                    return true;

                case "Cancel reservation":
                    
                    ShowUserMenu();
                    return false;

                default:
                    return false;
            }
        }
    }
   
    return false;
}


     private void ShowUserReservations()
     {
         Console.Clear();
         int UserId=_authService.GetLoggedUser().Id;
         using (var contex = new DatabaseContext())
         {
             var reservations = contex.Reservations
                 .Where(r => r.UserId == UserId)
                 .Select(r=>new {r.Details,r.Date})
                 .ToList();
             foreach (var reservation in reservations)
             {
                 Console.WriteLine($"Your reservation details: {reservation.Details}, Date: {reservation.Date}");
             }
             
         }
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
     }
}