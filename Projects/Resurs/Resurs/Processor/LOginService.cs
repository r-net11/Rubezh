using Common;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Resurs.Processor
{
	public static class LoginService
	{
		public static string Login(string login, string password)
		{

			return DbCache.CheckLogin(login,password);
		}

		public static void RestartApplication()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
			};
			System.Diagnostics.Process.Start(processStartInfo);
			System.Environment.Exit(1);
		}
	}
}