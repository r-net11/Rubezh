using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class UserRole
	{
		public UserRole()
		{
			UID = Guid.NewGuid();
			PermissionStrings = new List<string>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<string> PermissionStrings { get; set; }
	}
}