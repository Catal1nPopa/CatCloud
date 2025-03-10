namespace Application.DTOs.Folder
{
    public class FolderDTO
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid OwnerId { get; set; }
    }
}
