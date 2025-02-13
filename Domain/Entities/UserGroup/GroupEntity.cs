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
        public double TotalSpace { get; set; }
        public double AvailableSpace { get; set; }
        public DateTime Created { get; set; }

        public UserEntity Owner { get; set; }

        public ICollection<UserEntity> UserEntities { get; set; }
        //public ICollection<UserGroupRoleEntity> UserGroupRoles { get; set; }
        public ICollection<FileEntity> UploadedFiles { get; set; }
        public ICollection<RoleEntity> Rols { get; set; }
        //public ICollection<FileGroupShareEntity> SharedFiles { get; set; }
    }
}
