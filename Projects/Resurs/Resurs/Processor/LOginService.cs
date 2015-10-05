using Common;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public static class LoginService
	{
		public static string Login(string login, string password)
		{

			return DBCash.CheckLogin(login,password);
		}
	}
}