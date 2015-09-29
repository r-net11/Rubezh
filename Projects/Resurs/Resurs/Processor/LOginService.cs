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
			//using(var context = DatabaseContext.Initialize())
			//{
			//	var check = context.Users.Any(x=> x.PasswordHash == HashHelper.GetHashFromString(password) && x.Login == login);
			//	if (!check)
			//		return "неверный логин или пароль";
			//}
			return null;
		}
	}
}