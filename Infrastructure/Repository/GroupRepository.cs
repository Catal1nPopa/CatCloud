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
            _dbContext.Groups.Add(groupEntity);
            await _dbContext.SaveChangesAsync();
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
    }
}
