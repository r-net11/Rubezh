using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class User : ModelBase
	{
		public string Name { get; set;}

		public string Login { get; set; }

		public string Password { get; set; }

		public string PasswordHash { get; set; }

		public List<string> PermissionStrings { get; set; }
	}
}
