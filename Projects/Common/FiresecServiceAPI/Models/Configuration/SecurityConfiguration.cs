using System;
using System.Collections.Generic;
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
				if (userRole.UID == Guid.Empty)
				{
					userRole.UID = Guid.NewGuid();
					result = false;
				}
				if (userRole.PermissionStrings == null)
				{
					userRole.PermissionStrings = new List<string>();
					result = false;
				}
			}
			foreach (var user in Users)
			{
				if (user.UID == Guid.Empty)
				{
					user.UID = Guid.NewGuid();
					result = false;
				}
				if (user.PermissionStrings == null)
				{
					user.PermissionStrings = new List<string>();
					result = false;
				}
				if (user.OrganisationUIDs == null)
				{
					user.OrganisationUIDs = new List<Guid>();
					result = false;
				}
			}
			return result;
		}
	}
}