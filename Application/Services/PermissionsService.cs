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
            try
            {
                await _permissionsRepository.AddPermission(permission);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la adaugare permisiune noua {ex}");
            }
        }

        public async Task AddRole(string roleName)
        {
            try
            {
                await _permissionsRepository.AddRole(roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la adaugare rol nou {ex}");
            }
        }

        public async Task AssignPermissionsToRole(string role, List<string> permisions)
        {
            try
            {
                await _permissionsRepository.AssignPermissionsToRole(role, permisions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la asignare permisiuni la un rol {ex}");
            }
        }

        public async Task AssignRoleToUser(Guid userId, string roleName)
        {
            try
            {
                await _permissionsRepository.AssignRoleToUser(userId, roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la asignare rol pentru utilizator");
            }
        }

        public async Task AssignRoleToUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            try
            {
                await _permissionsRepository.AssignRoleToUserInGroup(userId, groupId, roleName);
            }
            catch(Exception ex)
            {
                throw new Exception($"Eroare la asignare rol |{roleName}| utilizatorului |{userId}| in grupul |{groupId}|");
            }
        }

        public async Task DeletePermission(string permission)
        {
            try
            {
                await _permissionsRepository.DeletePermission(permission);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la stergere permisiune {ex}");
            }
        }

        public async Task DeleteRole(string roleName)
        {
            try
            {
                await _permissionsRepository.DeleteRole(roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la stergere rol {ex}");
            }
        }

        public async Task<List<string>> GetPermissions()
        {
            try
            {
                return await _permissionsRepository.GetPermissions();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinere permisiuni {ex}");
            }
        }

        public async Task<List<string>> GetRolePermissions(string roleName)
        {
            try
            {
                return await _permissionsRepository.GetRolePermissions(roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinerea permisiuni la un rol {ex}");
            }
        }

        public async Task<List<string>> GetRoles()
        {
            try
            {
                return await _permissionsRepository.GetRoles();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinerea rolurilor {ex}");
            }
        }

        public async Task<List<string>> GetUserRoles(Guid userId)
        {
            try
            {
                return await _permissionsRepository.GetUserRoles(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinerea rolurilor utilizatorului {userId} - {ex}");
            }
        }

        public async Task<List<string>> GetUserRolesInGroup(Guid userId, Guid groupId)
        {
            try
            {
                return await _permissionsRepository.GetUserRolesInGroup(userId, groupId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obtinerea rolurilor utilizatorului |{userId}| in grupul |{groupId}|");
            }
        }

        public async Task RemoveRoleFromUser(Guid userId, string roleName)
        {
            try
            {
                await _permissionsRepository.RemoveRoleFromUser(userId, roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la stergerea rolului {roleName}, pentru utilizatorul {userId} - {ex}");
            }
        }

        public async Task RemoveRoleFromUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            try
            {
                await _permissionsRepository.RemoveRoleFromUserInGroup(userId, groupId, roleName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la stergere rol |{roleName}| utilizatorului |{userId}| in grupul |{groupId}|");
            }
        }
    }
}
