using System;

namespace CatCloud.Models.Group
{
    public class GroupFilesMetadataModel
    {
        public Guid FileId { get; set; }
        public string OwnerFile { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
    }
}
