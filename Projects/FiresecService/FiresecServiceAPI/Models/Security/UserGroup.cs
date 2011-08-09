using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class UserGroup
    {
        public UserGroup()
        {
            Permissions = new List<string>();
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<string> Permissions { get; set; }
    }
}