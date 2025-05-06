using System;
using System.Collections.Generic;

namespace CatCloud.Models.Group
{
    public class UserToGroupModel
    {
        public List<Guid> UserIds { get; set; }
        public Guid GroupId { get; set; }
    }
}
