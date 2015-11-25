using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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
		private const string ConfigDir = "C:\\ProgramData\\Firesec2";
		private const string ConfigFile = "AppServerSettings.xml";
		
		private const string SqlServerAddress = "SQLSERVER_ADDRESS";
		private const string SqlServerPort = "SQLSERVER_PORT";
		private const string SqlServerInstanceName = "SQLSERVER_INSTANCENAME";
		private const string SqlServerAuthenticationMode = "SQLSERVER_AUTHENTICATIONMODE";
		private const string SqlServerLogin = "SQLSERVER_LOGIN";
		private const string SqlServerPassword = "SQLSERVER_PASSWORD";

		private const string SqlServerAddressDefault = ".";
		private const int SqlServerPortDefault = 1433;
		private const string SqlServerInstanceNameDefault = "SQLEXPRESS";
		private const bool SqlServerAuthenticationModeDefault = true;
		private const string SqlServerLoginDefault = "strazh";
		private const string SqlServerPasswordDefault = "strazhstrazh";

		[CustomAction]
		public static ActionResult ReadAppServerSettings(Session session)
		{
			session.Log("Выполнение ReadAppServerSettings");

			try
			{
				session.Log("Каталог конфигурации: [{0}]", ConfigDir);

				var root = ReadConfigRoot(session, ConfigDir);
				if (root == null)
				{
					return ActionResult.Success;
				}

				session[SqlServerAddress] = GetSqlServerAddress(root);
				session[SqlServerPort] = GetSqlServerPort(root).ToString();
				session[SqlServerInstanceName] = GetSqlServerInstanceName(root);
				session[SqlServerAuthenticationMode] = GetSqlServerAuthenticationMode(root) ? Convert.ToString(0) : Convert.ToString(1);
				session[SqlServerLogin] = GetSqlServerLogin(root);
				session[SqlServerPassword] = GetSqlServerPassword(root);
				//MessageBox.Show(
				//	String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}",
				//	session[SqlServerAddress],
				//	session[SqlServerPort],
				//	session[SqlServerInstanceName],
				//	session[SqlServerAuthenticationMode],
				//	session[SqlServerLogin],
				//	session[SqlServerPassword]));
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения ReadAppServerSettings возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		private static XElement ReadConfigRoot(Session session, string location)
		{
			if (!Directory.Exists(location))
			{
				session.Log("Каталог установки [{0}] не найден", location);
				return null;
			}

			var configFile = Path.Combine(location, ConfigFile);
			if (!File.Exists(configFile))
			{
				session.Log("Конфигурационный файл [{0}] не найден", configFile);
				return null;
			}

			XDocument doc = null;
			using (var reader = new StreamReader(configFile))
			{
				doc = XDocument.Load(reader);
			}

			return (doc == null) ? null : doc.Root;
		}

		private static string GetSqlServerAddress(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBServerAddress");
			if (element == null)
				return SqlServerAddressDefault;

			return element.Value;
		}

		private static int GetSqlServerPort(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBServerPort");
			if (element == null)
				return SqlServerPortDefault;

			int result;
			if (!int.TryParse(element.Value, out result))
				return SqlServerPortDefault;

			return result;
		}

		private static string GetSqlServerInstanceName(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBServerName");
			if (element == null)
				return SqlServerInstanceNameDefault;

			return element.Value;
		}

		private static bool GetSqlServerAuthenticationMode(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBUseIntegratedSecurity");
			if (element == null)
				return SqlServerAuthenticationModeDefault;

			bool result;
			if (!Boolean.TryParse(element.Value, out result))
				return SqlServerAuthenticationModeDefault;
			return result;
		}

		private static string GetSqlServerLogin(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBUserID");
			if (element == null)
				return SqlServerLoginDefault;

			return element.Value;
		}

		private static string GetSqlServerPassword(XElement root)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == "DBUserPwd");
			if (element == null)
				return SqlServerPasswordDefault;

			return element.Value;
		}

		#endregion </Чтение файла конфигурации AppServerSettings.xml>
	}
}