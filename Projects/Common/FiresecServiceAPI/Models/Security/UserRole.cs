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
			PermissionStrings = new List<string>();
			UID = Guid.NewGuid();
        }

		[DataMember]
		public Guid UID { get; set; }

        [DataMember]
        public string Name { get; set; }

		[DataMember]
		public List<string> PermissionStrings { get; set; }
    }
}