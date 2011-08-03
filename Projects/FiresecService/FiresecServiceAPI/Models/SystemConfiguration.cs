using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class SystemConfiguration
    {
        public SystemConfiguration()
        {
            JournalFilters = new List<JournalFilter>();
            Users = new List<User>();
            UserGroups = new List<UserGroup>();
            Perimissions = new List<Perimission>();
        }

        [DataMember]
        public List<JournalFilter> JournalFilters { get; set; }

        [DataMember]
        public List<User> Users { get; set; }

        [DataMember]
        public List<UserGroup> UserGroups { get; set; }

        [DataMember]
        public List<Perimission> Perimissions { get; set; }
    }
}
