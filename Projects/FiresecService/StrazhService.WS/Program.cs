using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Common;

namespace StrazhService.WS
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] args)
		{
			if (args != null && args.Length == 1 && args[0].Length > 1
				&& (args[0][0] == '-' || args[0][0] == '/'))
			{
				var param = args[0].Substring(1).ToLower();
				string msg;
				switch (param)
				{
					case "install":
					case "i":
						if (!ServiceInstallerUtility.Install())
						{
							msg = "Ошибка регистрации сервиса";
							Logger.Error(msg);
							Console.WriteLine(msg);
						}
						break;
					case "uninstall":
					case "u":
						if (!ServiceInstallerUtility.Uninstall())
						{
							msg = "Ошибка деинсталляции сервиса";
							Logger.Error(msg);
							Console.WriteLine(msg);
						}
						break;
					default:
						msg = string.Format("Нераспознанный параметр '{0}'", param);
						Logger.Error(msg);
						Console.WriteLine(msg);
						break;
				}
			}
			else
			{
				try
				{
					var service = new StrazhWindowsService();

					if (Environment.UserInteractive)
					{
						Console.WriteLine("Служба Сервера A.C.Tech");
						Console.WriteLine();
						Console.WriteLine("StrazhService [/i | /u]");
						Console.WriteLine();
						Console.WriteLine("/i\tУстановка Сервера A.C.Tech в системе в качестве службы");
						Console.WriteLine("/u\tУдаление Сервера A.C.Tech из списка служб системы");
						Console.WriteLine();

#if DEBUG
						Console.CancelKeyPress += (x, y) => service.DoStop();
						service.DoStart();
						Console.WriteLine("Сервер запущен. Нажмите Ctrl+C для остановки...");
						while (true)
						{
						}
#endif
					}
					else
					{
						var servicesToRun = new ServiceBase[] { service };
						ServiceBase.Run(servicesToRun);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Ошибка запуска Windows-службы 'StrazhService'");
				}
			}
		}
	}

}
