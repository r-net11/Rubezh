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
            Permisions = new List<PermissionType>();
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<PermissionType> Permisions { get; set; }

        [DataMember]
        public List<string> Permissions { get; set; }
    }
}