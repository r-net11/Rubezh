using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class UserRole
    {
        public UserRole()
        {
            Permissions = new List<PermissionType>();
        }

        [DataMember]
        public UInt64 Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<PermissionType> Permissions { get; set; }
    }
}