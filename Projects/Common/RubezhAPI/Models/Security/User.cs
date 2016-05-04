using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class User
	{
		public User()
		{
			UID = Guid.NewGuid();
			PermissionStrings = new List<string>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string PasswordHash { get; set; }

		[DataMember]
		public List<string> PermissionStrings { get; set; }

		[DataMember]
		public RemoteAccess RemoteAccess { get; set; }

		[DataMember]
		public bool IsAdm { get; set; }

		public bool HasPermission(PermissionType permissionType)
		{
			return IsAdm || PermissionStrings.Contains(permissionType.ToString());
		}
	}
}