using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class CashUser
	{
		public static List <User> Users { get; set; }

		static CashUser()
		{
			Users = new List<User>();
			for (int i = 0; i < 50; i++)
			{
				Users.Add(new User() { Name = "Name " + i, Login = "login" + i, });
			}
		}

	}
}
