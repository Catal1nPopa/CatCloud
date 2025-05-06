using System;

namespace CatCloud.Models.File
{
    public class CopyFileModel
    {
        public Guid fileId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid UserId { get; set; }
    }
}
