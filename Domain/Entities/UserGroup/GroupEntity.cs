namespace Domain.Entities.UserGroup
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalSpace { get; set; }
        public double AvailableSpace { get; set; }
        public DateTime Created { get; set; }

        public ICollection<UserGroupEntity> UserGroups { get; set; } = new List<UserGroupEntity>();
    }
}
