namespace Application.Interfaces
{
    public interface IPermissionsService
    {
        Task AddRole(string roleName);
        Task DeleteRole(string roleName);
        Task<List<string>> GetRoles();
        Task AddPermission(string permission);
        Task DeletePermission(string permission);
        Task<List<string>> GetPermissions();
        Task AssignPermissionsToRole(string role, List<string> permisions);
        Task<List<string>> GetRolePermissions(string roleName);
        Task AssignRoleToUser(Guid userId, string roleName);
        Task RemoveRoleFromUser(Guid userId, string roleName);
        Task<List<string>> GetUserRoles(Guid userId);
        Task RemoveRoleFromUserInGroup(Guid userId, Guid groupId, string roleName);
        Task<List<string>> GetUserRolesInGroup(Guid userId, Guid groupId);
        Task AssignRoleToUserInGroup(Guid userId, Guid groupId, string roleName);
    }
}
