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
            Sounds = new List<Sound>();
            JournalFilters = new List<JournalFilter>();
            Instructions = new List<Instruction>();
            Users = new List<User>();
            UserGroups = new List<UserGroup>();
            Perimissions = new List<Perimission>();
        }

        [DataMember]
        public List<Sound> Sounds { get; set; }

        [DataMember]
        public List<JournalFilter> JournalFilters { get; set; }

        [DataMember]
        public List<Instruction> Instructions { get; set; }

        [DataMember]
        public List<User> Users { get; set; }

        [DataMember]
        public List<UserGroup> UserGroups { get; set; }

        [DataMember]
        public List<Perimission> Perimissions { get; set; }
    }
}
