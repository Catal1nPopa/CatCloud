using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task CreateGroup(GroupEntity groupEntity);
        Task DeleteGroup(Guid groupId);
        Task EditGroup(GroupEntity updatedGroup);
        Task LinkUserToGroup(LinkUserToGroupEntity data);
        Task UnlinkUserFromGroup(LinkUserToGroupEntity data);
        Task<List<GroupEntity>> GetUserGroups(Guid userId);
        Task<List<UserEntity>> GetGroupUsers(Guid groupId);
        Task<List<GroupEntity>> GetGroupsNotSharedWithFile(Guid fileId, Guid userId);
        Task<List<UserEntity>> GetUsersToLink(Guid groupId);
        Task<GroupEntity> GetGroup(Guid groupId);
    }
}
