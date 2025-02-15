 using projekt_po.Services;

 public class UserMenu
 {
     private readonly UserService _userService;

     public UserMenu(UserService userService)
     {
         _userService = userService;
     }

     public void Showmenu()
     {
         _userService.ListUsers();
     }
 }