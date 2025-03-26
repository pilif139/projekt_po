using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Services;

/// <summary>
/// Enum representing different permissions that can be assigned to roles.
/// It is a flag enum, so it can be combined with bitwise OR operation.
/// </summary>
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
    private readonly IAuthService _authService;
    // dictionary with roles and their permissions
    private readonly Dictionary<Role, Permissions> _rolePermissions = new()
    {
        {Role.None, Permissions.None},
        {Role.Admin, Permissions.All},
        {Role.Worker, Permissions.Read | Permissions.Write},
        {Role.Client, Permissions.Read}
    };

    public RbacService(IAuthService authService, ILogger logger) : base(logger)
    {
        _authService = authService;
    }

    /// <summary>
    /// Checks if the current logged-in user has a role with the required permissions.
    /// </summary>
    /// <param name="permission">The permission from the Permissions enum.</param>
    /// <returns>True if the user has the required permissions, otherwise false.</returns>
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