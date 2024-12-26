using Domain.Entities.UserGroup;

namespace Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task CreateGroup(GroupEntity groupEntity);
        Task DeleteGroup(Guid groupId);
        Task LinkUserToGroup(LinkUserToGroupEntity data);
        Task UnlinkUserFromGroup(LinkUserToGroupEntity data);
    }
}
