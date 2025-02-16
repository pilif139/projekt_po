using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Services;

[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    All = Read | Write | Delete
}

public interface IRbacService
{
    bool CheckPermissions(Permissions permission);
}

public class RbacService : BaseService, IRbacService
{
    private readonly AuthService _authService;
    private readonly Dictionary<Role, Permissions> _rolePermissions = new()
    {
        {Role.Admin, Permissions.All},
        {Role.Worker, Permissions.Read | Permissions.Write},
        {Role.Client, Permissions.Read}
    };
    
    public RbacService(AuthService authService, ILogger logger) : base(logger)
    {
        _authService = authService;
    }
    
    public bool CheckPermissions(Permissions permission)
    {
        if (!_authService.IsUserLogged())
        {
            Log("Checked permissions without logging in.");
            return false;
        }

        Role role = _authService.GetLoggedUserRole();
        if (!_rolePermissions.ContainsKey(role))
        {
            Log($"Role {role} not found in permissions dictionary.");
            return false;
        }
        
        // bitwise AND operation to check if user has required permissions
        if ((_rolePermissions[role] & permission) == 0)
        {
            Log($"User doesn't have required permissions to execute method.");
            return false;
        }
        
        Log($"User has required permissions to execute method.");
        return true;
    }
    
}