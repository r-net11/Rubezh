using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecClient.Models
{
    public class User
    {
        public User()
        {
            Groups = new List<string>();
            Permissions = new List<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsBuiltIn { get; set; }
        public List<string> Groups { get; set; }
        public List<string> Permissions { get; set; }
    }
}
