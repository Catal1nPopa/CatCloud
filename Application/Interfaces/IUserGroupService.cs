using Application.DTOs.UserGroup;

namespace Application.Interfaces
{
    public interface IUserGroupService
    {
        Task CreateGroup(GroupDTO groupEntity);
        Task DeleteGroup(Guid groupId);
        Task LinkUserToGroup(UserToGroupDTO data);
        Task UnlinkUserFromGroup(UserToGroupDTO data);
    }
}
