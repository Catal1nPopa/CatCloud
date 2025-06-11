using Domain.Entities.Auth;
using Domain.Entities.UserGroup;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class GroupRepository(CloudDbContext dbContext) : IGroupRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;
        public async Task CreateGroup(GroupEntity groupEntity)
        {
            try
            {

            if (await _dbContext.Groups.AnyAsync(g => g.Name == groupEntity.Name))
            {
                throw new Exception($"Eroare la creare grup, grup existent");
            }

            var owner = await _dbContext.Users.FindAsync(groupEntity.OwnerId);
            if (owner == null)
            {
                throw new Exception($"Utilizatorul nu există");
            }

            groupEntity.UserEntities = new List<UserEntity> { owner };

            _dbContext.Groups.Add(groupEntity);
            await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task DeleteGroup(Guid groupId)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                _dbContext.Groups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Grupul nu a fost găsit.");
            }
        }

        public async Task EditGroup(GroupEntity updatedGroup)
        {
            try
            {
                var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == updatedGroup.Id);

                if (group != null)
                {
                    var existingGroup = await _dbContext.Groups
                        .FirstOrDefaultAsync(g => g.Name == updatedGroup.Name);

                    if (existingGroup != null)
                    {
                        throw new Exception("Numele grupului este deja utilizat.");
                    }

                    group.Name = updatedGroup.Name;
                    group.Description = updatedGroup.Description;

                    _dbContext.Groups.Update(group);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Grupul nu a fost găsit.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la editarea grupului: {ex.Message}");
            }
        }


        public async Task LinkUserToGroup(LinkUserToGroupEntity data)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId);
            if (group == null)
            {
                throw new Exception("Grupul nu există.");
            }

            var users = await _dbContext.Users.Where(u => data.UserIds.Contains(u.Id)).ToListAsync();
            if (!users.Any())
            {
                throw new Exception("Niciun utilizator valid nu a fost găsit.");
            }

            var existingUserGroups = await _dbContext.UserGroups
                .Where(ug => ug.GroupId == data.GroupId && data.UserIds.Contains(ug.UserId))
                .Select(ug => ug.UserId)
                .ToListAsync();

            var newUserGroups = users
                .Where(u => !existingUserGroups.Contains(u.Id))
                .Select(u => new UserGroupEntity
                {
                    UserId = u.Id,
                    GroupId = group.Id
                })
                .ToList();

            if (newUserGroups.Any())
            {
                try
                {
                    await _dbContext.UserGroups.AddRangeAsync(newUserGroups);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Eroare la adăugarea utilizatorilor în grup: {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Toți utilizatorii selectați sunt deja în acest grup.");
            }
        }


        public async Task UnlinkUserFromGroup(LinkUserToGroupEntity data)
        {
            try
            {

            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId);
            if (group == null)
            {
                throw new Exception("Grupul nu există.");
            }

            var userGroups = await _dbContext.UserGroups
                .Where(ug => data.UserIds.Contains(ug.UserId) && ug.GroupId == data.GroupId)
                .ToListAsync();

            if (!userGroups.Any())
            {
                throw new Exception("Niciun utilizator de eliminat nu a fost găsit în acest grup.");
            }

            var usersToRemove = userGroups
                .Where(ug => ug.UserId != group.OwnerId)
                .ToList();

            if (!usersToRemove.Any())
            {
                throw new Exception("Nu poți elimina owner-ul grupului din grup.");
            }

            _dbContext.UserGroups.RemoveRange(usersToRemove);
            await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<List<GroupEntity>> GetUserGroups(Guid userId)
        {
            try
            {
                var userGroups = await _dbContext.UserGroups
                    .Where(ug => ug.UserId == userId)
                    .Select(ug => ug.GroupId)
                    .ToListAsync();

                var groups = await _dbContext.Groups
                    .Where(g => userGroups.Contains(g.Id))
                    .ToListAsync();

                return groups;
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obținerea grupurilor utilizatorului: {ex.Message}");
            }
        }

        public async Task<List<UserEntity>> GetGroupUsers(Guid groupId)
        {
            try
            {
                var groupUsers = await _dbContext.UserGroups
                    .Where(ug => ug.GroupId == groupId)
                    .Select(ug => ug.UserId)
                    .ToListAsync();

                var users = await _dbContext.Users
                    .Where(u => groupUsers.Contains(u.Id))
                    .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la obținerea utilizatorilor grupului: {ex.Message}");
            }
        }

        public async Task<List<GroupEntity>> GetGroupsNotSharedWithFile(Guid fileId,Guid userId)
        {
            var userGroupIds = _dbContext.UserGroups
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GroupId);

            var sharedGroupIds = _dbContext.FileGroupShares
                .Where(fgs => fgs.FileId == fileId)
                .Select(fgs => fgs.GroupId);

            var availableGroups = await _dbContext.Groups
                .Where(g => userGroupIds.Contains(g.Id) && !sharedGroupIds.Contains(g.Id))
                .ToListAsync();
            return availableGroups;
        }

        public async Task<List<UserEntity>> GetUsersToLink(Guid groupId)
        {
            try
            {
                var usersToLink = await _dbContext.Users
                    .Where(u => !_dbContext.UserGroups
                    .Where(g => g.GroupId == groupId)
                    .Select(g => g.UserId)
                    .Contains(u.Id))
                    .ToListAsync();
                return usersToLink;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<GroupEntity> GetGroup(Guid groupId)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            return group;
        }
    }
}
