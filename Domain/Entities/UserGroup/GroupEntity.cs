using Domain.Entities.Auth;
using Domain.Entities.Files;
using Domain.Entities.Permission;

namespace Domain.Entities.UserGroup
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Created { get; set; }

        public UserEntity Owner { get; set; }

        public ICollection<UserEntity> UserEntities { get; set; }
        public ICollection<FileEntity> UploadedFiles { get; set; }
        public ICollection<UserGroupPermissionsEntity> UserGroupPermissions { get; set; }
    }
}
