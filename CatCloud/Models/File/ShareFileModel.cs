using System;
using System.Collections.Generic;

namespace CatCloud.Models.File
{
    public class ShareFileModel
    {
        public Guid fileId { get; set; }
        public List<Guid> objectIds { get; set; }
    }
}
