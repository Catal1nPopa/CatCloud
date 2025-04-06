namespace CatCloud.Models.Group
{
    public class CreateGroupModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
    }
}
