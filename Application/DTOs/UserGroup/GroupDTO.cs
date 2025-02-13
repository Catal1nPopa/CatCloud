namespace Application.DTOs.UserGroup
{
    public class GroupDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public double TotalSpace { get; set; }
        public double AvailableSpace { get; set; }
        public Guid OwnerId { get; set; }
    }
}
