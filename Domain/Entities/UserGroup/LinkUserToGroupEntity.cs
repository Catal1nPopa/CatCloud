namespace Domain.Entities.UserGroup
{
    public class LinkUserToGroupEntity
    {
        public List<Guid> UserIds { get; set; }
        public Guid GroupId { get; set; }
    }
}
