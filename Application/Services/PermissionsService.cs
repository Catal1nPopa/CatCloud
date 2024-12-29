using Application.Interfaces;
using Domain.Interfaces;
using System.Data;
using System.Security;

namespace Application.Services
{
    public class PermissionsService(IPermissionsRepository permissionsRepository) : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;
        public async Task AddPermission(string permission)
        {
            await _permissionsRepository.AddPermission(permission);
        }

        public async Task AddRole(string roleName)
        {
            await _permissionsRepository.AddRole(roleName);
        }

        public async Task AssignPermissionsToRole(string role, List<string> permisions)
        {
            await _permissionsRepository.AssignPermissionsToRole(role, permisions);
        }

        public async Task AssignRoleToUser(Guid userId, string roleName)
        {
            await _permissionsRepository.AssignRoleToUser(userId, roleName);
        }

        public async Task AssignRoleToUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            await _permissionsRepository.AssignRoleToUserInGroup(userId, groupId, roleName);
        }

        public async Task DeletePermission(string permission)
        {
            await _permissionsRepository.DeletePermission(permission);
        }

        public async Task DeleteRole(string roleName)
        {
            await _permissionsRepository.DeleteRole(roleName);
        }

        public async Task<List<string>> GetPermissions()
        {
            return await _permissionsRepository.GetPermissions();
        }

        public async Task<List<string>> GetRolePermissions(string roleName)
        {
            return await _permissionsRepository.GetRolePermissions(roleName);
        }

        public async Task<List<string>> GetRoles()
        {
            return await _permissionsRepository.GetRoles();
        }

        public async Task<List<string>> GetUserRoles(Guid userId)
        {
            return await _permissionsRepository.GetUserRoles(userId);
        }

        public async Task<List<string>> GetUserRolesInGroup(Guid userId, Guid groupId)
        {
            return await _permissionsRepository.GetUserRolesInGroup(userId, groupId);
        }

        public async Task RemoveRoleFromUser(Guid userId, string roleName)
        {
            await _permissionsRepository.RemoveRoleFromUser(userId, roleName);
        }

        public async Task RemoveRoleFromUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            await _permissionsRepository.RemoveRoleFromUserInGroup(userId, groupId, roleName);
        }
    }
}
