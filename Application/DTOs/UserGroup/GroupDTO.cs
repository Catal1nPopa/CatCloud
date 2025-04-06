namespace Application.DTOs.UserGroup
{
    public class GroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public Guid OwnerId { get; set; }
    }
}
