namespace CatCloud.Models.File
{
    public class FileUploadModel
    {
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public long FileSize { get; set; }
        public Guid UploadedByUserId { get; set; }

        public FileUploadModel(string name, DateTime uploaded, long fileSize, Guid userId)
        {
            FileName = name;
            UploadedAt = uploaded;
            FileSize = fileSize;
            UploadedByUserId = userId;
        }
    }
}
