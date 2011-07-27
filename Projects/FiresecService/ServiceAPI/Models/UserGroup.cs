using System.Collections.Generic;

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
