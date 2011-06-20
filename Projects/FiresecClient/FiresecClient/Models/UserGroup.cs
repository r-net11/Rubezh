using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecClient.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            Permissions = new List<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}
