namespace Application.DTOs.Files
{
    public class CopyFileDTO
    {
        public Guid fileId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid UserId { get; set; }
    }
}
