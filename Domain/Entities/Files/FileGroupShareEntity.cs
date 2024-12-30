using Domain.Entities.UserGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Files
{
    public class FileGroupShareEntity
    {
        public Guid FileId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime SharedAt { get; set; }

        public FileEntity File { get; set; }
        public GroupEntity Group { get; set; }
    }
}
