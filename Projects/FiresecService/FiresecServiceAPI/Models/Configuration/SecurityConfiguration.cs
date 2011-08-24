using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class SecurityConfiguration
    {
        public SecurityConfiguration()
        {
            Users = new List<User>();
            UserGroups = new List<UserGroup>();
            Perimissions = new List<Perimission>();
        }

        [DataMember]
        public List<User> Users { get; set; }

        [DataMember]
        public List<UserGroup> UserGroups { get; set; }

        [DataMember]
        public List<Perimission> Perimissions { get; set; }
    }
}
