using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class User 
	{
		public User()
		{
			UID = Guid.NewGuid();
			PermissionStrings = new List<string>();
		}
		public Guid UID { get; set; }
		public string Name { get; set;}

		public string Login { get; set; }

		public string Password { get; set; }

		public string PasswordHash { get; set; }

		public List<string> PermissionStrings { get; set; }

		public bool IsViewDevice { get; set; }

		public bool IsEditDevice { get; set; }

		public bool IsViewApartment { get; set; }

		public bool IsEditApartment { get; set; }

		public bool IsViewUser { get; set; }

		public bool IsEditUser { get; set; }


	}
}
