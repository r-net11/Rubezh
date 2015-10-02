using ResursAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class User 
	{
		public User()
		{
			UID = Guid.NewGuid();
			UserPermissions = new List<UserPermission>();
		}
		[Key]
		public Guid UID { get; set; }
		[MaxLength (100)]
		public string Name { get; set;}
		[MaxLength(100)]
		public string Login { get; set; }
		[MaxLength(200)]
		public string PasswordHash { get; set; }
		public List<UserPermission> UserPermissions { get; set; }

	}
}
