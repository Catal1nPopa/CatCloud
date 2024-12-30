using Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Files
{
    public class FileUserShareEntity
    {
        public Guid FileId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SharedAt { get; set; }

        public FileEntity File { get; set; }
        public UserEntity User { get; set; }
    }
}
