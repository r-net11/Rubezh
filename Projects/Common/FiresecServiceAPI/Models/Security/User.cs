using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class User
    {
        public User()
        {
            Permissions = new List<PermissionType>();
        }

        [DataMember]
        public UInt64 Id { get; set; }

        [DataMember]
        public UInt64 RoleId { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public List<PermissionType> Permissions { get; set; }

        [DataMember]
        public RemoteAccess RemoreAccess { get; set; }
    }
}