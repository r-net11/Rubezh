using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Enums;

namespace FiresecAPI.Models
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
		public RemoteAccess RemoreAccess { get; set; }

		public bool HasPermission(PermissionType permissionType)
		{
			return PermissionStrings.Contains(permissionType.ToString());
		}

		/// <summary>
		/// Используемая оболочка рабочего стола
		/// </summary>
		[DataMember]
		public ShellType ShellType { get; set; }
	}
}