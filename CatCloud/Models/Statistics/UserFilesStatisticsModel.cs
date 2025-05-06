using System;

namespace CatCloud.Models.Statistics
{
    public class UserFilesStatisticsModel
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
