using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Services;

/// <summary>
/// Enum representing different permissions that can be assigned to roles.
/// It is a flag enum, so it can be combined with bitwise OR operation.
/// </summary>
[Flags]
public enum Permission
{
    None = 0,
    Read = 1,
    Create = 2,
    Update = 4,
    Delete = 8,
    All = Read | Create | Update | Delete
}

public enum Resource
{
    User,
    Reservation
}

public interface IRbacService
{
    bool CheckPermission(Resource resource, Permission permission);
    bool CheckPermission<T>(Resource resource, Permission permission, T obj) where T : IModelType;
}

public class RbacService : BaseService, IRbacService
{
    private readonly IAuthService _authService;
    // dictionary with roles and their permissions
    private readonly Dictionary<Role, Dictionary<Resource, Permission>> _rolePermissions = new()
    {
        {
            Role.Admin, new Dictionary<Resource, Permission>
            {
                { Resource.User, Permission.All },
                { Resource.Reservation, Permission.All }
            }
        },
        {
            Role.Worker, new Dictionary<Resource, Permission>
            {
                { Resource.User, Permission.Read },
                { Resource.Reservation, Permission.All }
            }
        },
        {
            Role.Client, new Dictionary<Resource, Permission>
            {
                { Resource.User, Permission.Read },
                { Resource.Reservation, Permission.Read | Permission.Create }
            }
        },
        {
            Role.None, new Dictionary<Resource, Permission>
            {
                { Resource.User, Permission.None },
                { Resource.Reservation, Permission.None }
            }
        }
    };

    public RbacService(IAuthService authService, ILogger logger) : base(logger)
    {
        _authService = authService;
    }

    /// <summary>
    /// Checks if the current logged-in user has a role with the required permissions for specific resource like user or reservation.
    /// </summary>
    /// <param name="permission">The permission from the Permission enum.</param>
    /// <param name="resource">The resource from the Resource enum.</param>
    /// <returns>True if the user has the required permissions, otherwise false.</returns>
    public bool CheckPermission(Resource resource, Permission permission)
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

        if (_rolePermissions.TryGetValue(role, out var resourcePermissions) &&
            resourcePermissions.TryGetValue(resource, out var allowedPermissions))
        {
            if ((allowedPermissions & permission) == permission)
            {
                Log("User has permission: " + permission);
                return true;
            }
            Log("User does not have permission: " + permission);
        }
        else
        {
            Log($"Resource {resource} not found in permissions dictionary for role {role}.");
        }
        return false;
    }

    /// <summary>
    /// Checks if the current logged in user has permission for specific object.
    /// <param name="resource">The resource from the Resource enum.</param>
    /// <param name="permission">The permission from the Permission enum.</param>
    /// <param name="obj">Object to check permissions for</param>
    /// <returns>True if the user has the required permissions, otherwise false.</returns>
    /// </summary>
    public bool CheckPermission<T>(Resource resource, Permission permission, T obj) where T : IModelType
    {
        var loggedUser = _authService.GetLoggedUser()!;

        if (loggedUser.Role != Role.Client)
        {
            return CheckPermission(resource, permission);
        }

        switch (obj)
        {
            case User user:
                Log("Requested user: " + user.Id + " logged user: " + loggedUser.Id);
                return loggedUser.Id == user.Id;
            case Reservation reservation:
                Log("Requested reservation: " + reservation.UserId + " logged user: " + loggedUser.Id);
                return loggedUser.Id == reservation.UserId;
        }

        Log($"User does not have permission: {permission}");
        return false;
    }

}