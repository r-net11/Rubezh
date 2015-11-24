using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;

namespace CustomAction
{
	public class CustomActions
	{
		#region <Закрытие приложений перед проведением обновления>

		[CustomAction]
		public static ActionResult CloseApplications(Session session)
		{
			session.Log("Выполнение CloseApplications");
			Process[] processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				try
				{
					if ((process.ProcessName == "FiresecService")
						|| (process.ProcessName == "FireMonitor")
						|| (process.ProcessName == "FireAdministrator")
						|| (process.ProcessName == "Revisor")
						|| (process.ProcessName == "GKOPCServer")
						|| (process.ProcessName == "FiresecNTService"))
					{
						process.Kill();
					}
				}
				catch(Exception e)
				{
					session.Log("В результате выполнения CloseApplications возникла ошибка: {0}", e.Message);
				}
			}
			return ActionResult.Success;
		}

		#endregion </Закрытие приложений перед проведением обновления>

		#region <Чтение файла конфигурации AppServerSettings.xml>

		private const string InstallLocation = "INSTALLLOCATION";
		private const string ConfigFile = "AppServerSettings.xml";
		private const string ServerPort = "SERVER_PORT";
		private const int DefaultPort = 8888;

		[CustomAction]
		public static ActionResult ReadAppServerSettings(Session session)
		{
			session.Log("Выполнение ReadAppServerSettings");

			try
			{
				var installLocation = session[InstallLocation];
				session.Log("Каталог установки: [{0}]", installLocation);

				var root = ReadConfigRoot(session, installLocation);
				if (root == null)
					return ActionResult.Success;

				int port = GetSqlServerPort(root);
				session[ServerPort] = port.ToString();
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения ReadAppServerSettings возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		private static XmlElement ReadConfigRoot(Session session, string installLocation)
		{
			if (!Directory.Exists(installLocation))
			{
				session.Log("Каталог установки [{0}] не найден", installLocation);
				return null;
			}

			string configFile = Path.Combine(installLocation, ConfigFile);
			if (!File.Exists(configFile))
			{
				session.Log("Конфигурационный файл [{0}] не найден", configFile);
				return null;
			}

			var doc = new XmlDocument();
			using (var reader = new StreamReader(configFile))
			{
				doc.Load(reader);
			}

			return doc.DocumentElement;
		}

		private static int GetSqlServerPort(XmlElement root)
		{
			const string path = "descendant::service[@name='ApplicationService.Common.ApplicationService']/host/baseAddresses/add[@baseAddress]/@baseAddress";

			const string portPattern = @"localhost:(?<port>\d*)";

			string address = LoadAttribute(root, path);
			if (string.IsNullOrEmpty(address))
				return DefaultPort;

			var regex = new Regex(portPattern);
			var matches = regex.Match(address);
			if (!matches.Success)
				return DefaultPort;

			var portString = matches.Groups["port"].Value;

			int result;
			if (!int.TryParse(portString, out result))
				return DefaultPort;

			return result;
		}

		private static string LoadAttribute(XmlElement root, string path)
		{
			var node = root.SelectSingleNode(path);
			return node != null ? node.Value : null;
		}

		#endregion </Чтение файла конфигурации AppServerSettings.xml>
	}
}