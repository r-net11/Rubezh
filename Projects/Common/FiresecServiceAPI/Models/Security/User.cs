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
			PermissionStrings = new List<string>();
			UID = Guid.NewGuid();
        }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid RoleUID { get; set; }

		[Obsolete]
        [DataMember]
        public UInt64 Id { get; set; }

		[Obsolete]
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
		public List<string> PermissionStrings { get; set; }

        [DataMember]
        public RemoteAccess RemoreAccess { get; set; }
    }
}