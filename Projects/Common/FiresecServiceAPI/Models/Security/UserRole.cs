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
			PermissionStrings = new List<string>();
			UID = Guid.NewGuid();
        }

		[DataMember]
		public Guid UID { get; set; }

		[Obsolete]
        [DataMember]
        public UInt64 Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<PermissionType> Permissions { get; set; }

		[DataMember]
		public List<string> PermissionStrings { get; set; }
    }
}