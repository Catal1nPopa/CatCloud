using Application.DTOs.Auth;
using Application.DTOs.UserGroup;
using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Application.Interfaces
{
    public interface IUserGroupService
    {
        Task CreateGroup(GroupDTO groupEntity);
        Task DeleteGroup(Guid groupId);
        Task EditGroup(GroupDTO newGroupData);
        Task LinkUserToGroup(UserToGroupDTO data);
        Task UnlinkUserFromGroup(UserToGroupDTO data);
        Task<List<UserInfoDTO>> GetGroupUsers(Guid groupId);
        Task<List<GroupDTO>> GetUserGroups();
        Task<List<GroupDTO>> GetGroupsNotSharedWithFile(Guid fileId);
        Task<List<UserInfoDTO>> GetUsersToLink(Guid groupId);   
    }
}
