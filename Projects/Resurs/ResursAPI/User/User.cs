using ResursAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class User : ModelBase
	{
		public User()
		{
			UserPermissions = new List<UserPermission>();
		}

		[MaxLength (100)]
		public string Login { get; set; }
		[MaxLength(200)]
		public string PasswordHash { get; set; }
		public List<UserPermission> UserPermissions { get; set; }

	}
}