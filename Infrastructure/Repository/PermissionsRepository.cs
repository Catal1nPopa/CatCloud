using Domain.Entities.Permission;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class PermissionsRepository(CloudDbContext dbContext) : IPermissionsRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;

        public async Task AddPermission(string permission) //lucreaza
        {
            var existingPermission = await _dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            if (existingPermission == null)
            {
                var newPermission = new PermissionEntity
                {
                    Id = Guid.NewGuid(),
                    Description = "s",
                    Name = permission
                };

                _dbContext.Permissions.Add(newPermission);
                await _dbContext.SaveChangesAsync();
            }
            else
                throw new Exception($"Permisiunea data deja a fost creata");
        }

        public async Task AddRole(string roleName)  //lucreaza
        {
            try
            {

                var existingRole = await _dbContext.Roles
                    .FirstOrDefaultAsync(r => r.Name == roleName);

                if (existingRole == null)
                {
                    var newRole = new RoleEntity
                    {
                        Id = Guid.NewGuid(),
                        Description = "tres",
                        Name = roleName
                    };

                    _dbContext.Roles.Add(newRole);
                    await _dbContext.SaveChangesAsync();
                }
                else
                    throw new ArgumentException($"Rolul dat deja a fost creat");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignPermissionsToRole(string roleName, List<string> permissionNames) //lucreaza
        {
            var role = await _dbContext.Roles
                .Include(r => r.RolePermissions) // Asigură includerea relațiilor
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
                throw new ArgumentException($"Rolul '{roleName}' nu există.");

            // Căutăm permisiunile existente după nume
            var permissions = await _dbContext.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();

            if (permissions.Count == 0)
                throw new ArgumentException("Nicio permisiune validă nu a fost găsită.");

            // Verificăm permisiunile deja atribuite acestui rol
            var existingPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

            // Creăm noi intrări doar pentru permisiunile care nu sunt deja atribuite
            var newRolePermissions = permissions
                .Where(p => !existingPermissionIds.Contains(p.Id))
                .Select(p => new RolePermissionEntity
                {
                    RoleId = role.Id,
                    PermissionId = p.Id
                })
                .ToList();

            if (newRolePermissions.Any())
            {
                await _dbContext.RolePermissions.AddRangeAsync(newRolePermissions);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Toate permisiunile specificate sunt deja atribuite rolului '{roleName}'.");
            }
        }


        public async Task DeletePermission(string permission)
        {
            var existingPermission = await _dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            if (existingPermission != null)
            {
                _dbContext.Permissions.Remove(existingPermission);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteRole(string roleName)
        {
            var existingRole = await _dbContext.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (existingRole != null)
            {
                _dbContext.Roles.Remove(existingRole);
                await _dbContext.SaveChangesAsync();
            }
            else
                throw new Exception($"Rolul {roleName} nu exista");
        }

        public async Task<List<string>> GetPermissions()
        {
            return await _dbContext.Permissions
                .Select(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetRoles()
        {
            return await _dbContext.Roles
                .Select(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetRolePermissions(string roleName)
        {
            var role = await _dbContext.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                throw new Exception($"Rolul '{roleName}' nu există.");
            }

            return role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();
        }

        public async Task AssignRoleToUser(Guid userId, string roleName)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (user == null)
            {
                throw new Exception("Utilizatorul nu a fost găsit.");
            }
            if (role == null)
            {
                throw new Exception($"Rolul '{roleName}' nu există.");
            }

            if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                throw new Exception($"Utilizatorul are deja rolul '{roleName}'.");
            }

            user.UserRoles.Add(new UserRoleEntity
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUser(Guid userId, string roleName)
        {
            var userRole = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);

            if (userRole == null)
            {
                throw new Exception($"Rolul '{roleName}' nu este asignat utilizatorului.");
            }

            _dbContext.UserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserRoles(Guid userId)
        {
            var roles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            return roles;
        }

        public async Task RemoveRoleFromUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            //var userGroupRole = await _dbContext.UserGroupRoles
            //    .Include(ugr => ugr.Role)
            //    .FirstOrDefaultAsync(ugr =>
            //        ugr.UserId == userId &&
            //        ugr.GroupId == groupId &&
            //        ugr.Role.Name == roleName
            //    );

            //if (userGroupRole == null)
            //{
            //    throw new Exception($"Rolul '{roleName}' nu este asignat utilizatorului în acest grup.");
            //}

            //_dbContext.UserGroupRoles.Remove(userGroupRole);
            //await _dbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }


        public async Task<List<string>> GetUserRolesInGroup(Guid userId, Guid groupId)
        {
            //var roles = await _dbContext.UserGroupRoles
            //    .Where(ugr => ugr.UserId == userId && ugr.GroupId == groupId)
            //    .Select(ugr => ugr.Role.Name)
            //    .ToListAsync();

            //return roles;
            throw new NotImplementedException();
        }

        public async Task AssignRoleToUserInGroup(Guid userId, Guid groupId, string roleName)
        {
            //var user = await _dbContext.Users
            //    .Include(u => u.UserGroupRoles)
            //    .FirstOrDefaultAsync(u => u.Id == userId);

            //var group = await _dbContext.Groups
            //    .FirstOrDefaultAsync(g => g.Id == groupId);

            //var role = await _dbContext.Roles
            //    .FirstOrDefaultAsync(r => r.Name == roleName);

            //if (user == null)
            //{
            //    throw new Exception("Utilizatorul nu a fost găsit.");
            //}
            //if (group == null)
            //{
            //    throw new Exception("Grupul nu a fost găsit.");
            //}
            //if (role == null)
            //{
            //    throw new Exception($"Rolul '{roleName}' nu există.");
            //}

            //var userGroup = await _dbContext.UserGroups
            //    .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

            //if (userGroup == null)
            //{
            //    // Adăugăm utilizatorul în grup daca nu este deja
            //    _dbContext.UserGroups.Add(new UserGroupEntity
            //    {
            //        UserId = userId,
            //        GroupId = groupId
            //    });
            //}

            //if (user.UserGroupRoles.Any(ugr => ugr.GroupId == groupId && ugr.RoleId == role.Id))
            //{
            //    throw new Exception($"Utilizatorul are deja rolul '{roleName}' în acest grup.");
            //}

            //user.UserGroupRoles.Add(new UserGroupRoleEntity
            //{
            //    UserId = userId,
            //    GroupId = groupId,
            //    RoleId = role.Id
            //});

            //await _dbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }
    }
}
