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
            if (await _dbContext.Groups.FirstOrDefaultAsync(u => u.Name == groupEntity.Name) == null)
            {
                _dbContext.Groups.Add(groupEntity);
                await _dbContext.SaveChangesAsync();
            }
            else
                throw new Exception($"Eroare la creare grup, grup existent");
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
                    group.TotalSpace = updatedGroup.TotalSpace;
                    group.AvailableSpace = updatedGroup.AvailableSpace;

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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == data.UserId);
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId);

            if (user != null && group != null)
            {
                var userGroup = new UserGroupEntity
                {
                    UserId = user.Id,
                    GroupId = group.Id
                };
                try
                {

                    _dbContext.UserGroups.Add(userGroup);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex}");
                }
            }
            else
                throw new Exception($"Utilizator sau grup nu exista");
        }

        public async Task UnlinkUserFromGroup(LinkUserToGroupEntity data)
        {
            var userGroup = await _dbContext.UserGroups
                .FirstOrDefaultAsync(ug => ug.UserId == data.UserId && ug.GroupId == data.GroupId);
            if (userGroup != null)
            {
                _dbContext.UserGroups.Remove(userGroup);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Utilizatorul nu este asociat cu acest grup.");
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
    }
}
