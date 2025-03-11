namespace Application.DTOs.Folder
{
    public class FolderDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid OwnerId { get; set; }
    }
}
