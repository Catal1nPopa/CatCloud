using Application.DTOs.UserGroup;
using Application.Interfaces;
using CatCloud.Models.Group;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController(IUserGroupService userGroupService) : ControllerBase
    {
        private readonly IUserGroupService _userGroupService = userGroupService;

        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup(GroupModel group)
        {
            await _userGroupService.CreateGroup(group.Adapt<GroupDTO>());
            return Ok();
        }

        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            await _userGroupService.DeleteGroup(groupId);
            return Ok();
        }

        [HttpPost("LinkUserToGroup")]
        public async Task<IActionResult> LinkUserToGroup(UserToGroupModel userToGroup)
        {
            await _userGroupService.LinkUserToGroup(userToGroup.Adapt<UserToGroupDTO>());
            return Ok();
        }

        [HttpPost("UnlinkUserToGroup")]
        public async Task<IActionResult> UnlinkUserToGroup(UserToGroupModel userToGroup)
        {
            await _userGroupService.UnlinkUserFromGroup(userToGroup.Adapt<UserToGroupDTO>());
            return Ok();
        }
    }
}
