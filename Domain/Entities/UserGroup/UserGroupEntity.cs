using Domain.Entities.Auth;

namespace Domain.Entities.UserGroup
{
    public class UserGroupEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }
    }
}
