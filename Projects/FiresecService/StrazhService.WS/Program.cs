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
				switch (args[0].Substring(1).ToLower())
				{
					case "install":
					case "i":
						if (!ServiceInstallerUtility.Install())
							Logger.Error("Ошибка регистрации сервиса");
							Console.WriteLine("Ошибка регистрации сервиса");
						break;
					case "uninstall":
					case "u":
						if (!ServiceInstallerUtility.Uninstall())
							Logger.Error("Ошибка деинсталляции сервиса");
							Console.WriteLine("Ошибка деинсталляции сервиса");
						break;
					default:
						Logger.Error("Нераспознанный параметр");
						Console.WriteLine("Нераспознанный параметр");
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
						//Console.OutputEncoding = Encoding.UTF8;

						Console.CancelKeyPress += (x, y) => service.Stop();
						service.DoStart();
						Console.WriteLine("Служба запущена. Нажмите любую клавишу для остановки...");
						Console.ReadKey();
						service.DoStop();
						Console.WriteLine("Служба остановлена.");
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
