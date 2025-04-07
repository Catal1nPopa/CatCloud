
namespace Application.DTOs.Statistics
{
    public class UserFilesStatisticsDTO
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
