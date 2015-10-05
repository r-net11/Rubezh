using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class UserPermission
	{
		public UserPermission()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }
		public PermissionType PermissionType { get; set; }
		public User User { get; set; }
	}
}
