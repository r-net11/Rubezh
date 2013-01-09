using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class SecurityConfiguration : VersionedConfiguration
	{
		public SecurityConfiguration()
		{
			Users = new List<User>();
			UserRoles = new List<UserRole>();
		}

		[DataMember]
		public List<User> Users { get; set; }

		[DataMember]
		public List<UserRole> UserRoles { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			foreach (var userRole in UserRoles)
			{
				if (userRole.PermissionStrings == null)
				{
					userRole.PermissionStrings = new List<string>();
					result = false;
				}
				if (userRole.UID == Guid.Empty)
				{
					userRole.UID = Guid.NewGuid();
					result = false;
				}
			}
			foreach (var user in Users)
			{
				if (user.PermissionStrings == null)
				{
					user.PermissionStrings = new List<string>();
					result = false;
				}
				if (user.UID == Guid.Empty)
				{
					user.UID = Guid.NewGuid();
					result = false;
					var userRole = UserRoles.FirstOrDefault(x => x.Id == user.RoleId);
					if (userRole != null)
					{
						user.RoleUID = userRole.UID;
					}
				}
			}
			foreach (var userRole in UserRoles)
			{
				userRole.Id = 0;
			}
			foreach (var user in Users)
			{
				user.RoleId = 0;
				user.Id = 0;
			}
			return result;
		}
	}
}