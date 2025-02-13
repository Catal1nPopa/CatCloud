namespace Application.DTOs.UserGroup
{
    public class UserToGroupDTO
    {
        public List<Guid> UserIds { get; set; }
        public Guid GroupId { get; set; }
    }
}
