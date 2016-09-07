using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Common;
using Localization.StrazhService.WS.Common;
using Localization.StrazhService.WS.Errors;

namespace StrazhService.WS
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] args)
		{
			var culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

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
							msg = CommonErrors.RegistrationService_Error;
							Logger.Error(msg);
							Console.WriteLine(msg);
						}
						break;
					case "uninstall":
					case "u":
						if (!ServiceInstallerUtility.Uninstall())
						{
							msg = CommonErrors.DeinstallationService_Error;
							Logger.Error(msg);
							Console.WriteLine(msg);
						}
						break;
					default:
						msg = string.Format(CommonResources.UnrecognisedParameter, param);
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
						// Подписываемся на все неотловленные исключения, включая те, что были сгенерированы не в основном потоке
						AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

						Console.WriteLine(CommonResources.ServerService);
						Console.WriteLine();
						Console.WriteLine("StrazhService [/i | /u]");
						Console.WriteLine();
						Console.WriteLine(CommonResources.InstallationService);
						Console.WriteLine(CommonResources.UninstallationService);
						Console.WriteLine();

#if DEBUG
						Console.CancelKeyPress += (x, y) => service.DoStop();
						service.DoStart();
						Console.WriteLine(CommonResources.ServerStarted);
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

		/// <summary>
		/// Обработать общую ошибку
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception exception = null;
			var o = args.ExceptionObject as Exception;
			if (o != null)
				exception = o;

			if (exception == null)
				exception = new Exception(CommonErrors.UnhandledException_Error);

			var errorMessage =
				String.Format(CommonErrors.MainThread_Error
							  + Environment.NewLine + "Exception.Message: {0}"
							  + Environment.NewLine + "Exception.Source: {1}"
							  + Environment.NewLine + "Exception.StackTrace: {2}",
					exception.Message, exception.Source, exception.StackTrace);

			errorMessage += exception.InnerExceptionToString();

			// Пишем сообщение в лог
			try
			{
				Logger.Error(errorMessage);
			}
			catch
			{

			}

			Trace.WriteLine(string.Format("{0}\t{1}", DateTime.Now, errorMessage));
		}
	}

}
