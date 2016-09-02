using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;

namespace CustomAction
{
	public class CustomActions
	{
		#region <Работа с файлом конфигурации AppServerSettings.xml>

		private const string ConfigDir = "C:\\ProgramData\\Strazh";
		private const string ConfigFile = "AppServerSettings.xml";

		#region <Переменные сессии и значения по умолчанию>

		private const string ProductName = "ProductName";
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

		#endregion </Переменные сессии и значения по умолчанию>

		private const string DBServerAddressNodeName = "DBServerAddress";
		private const string DBServerPortNodeName = "DBServerPort";
		private const string DBServerNameNodeName = "DBServerName";
		private const string DBUseIntegratedSecurityNodeName = "DBUseIntegratedSecurity";
		private const string DBUserIDNodeName = "DBUserID";
		private const string DBUserPwdNodeName = "DBUserPwd";

		[CustomAction]
		public static ActionResult ReadAppServerSettings(Session session)
		{
			session.Log("Выполнение ReadAppServerSettings");

			try
			{
				session.Log("Каталог хранения конфигурации: [{0}]", ConfigDir);

				var doc = ReadConfigFile(session, Path.Combine(ConfigDir, ConfigFile));
				var root = (doc == null) ? null : doc.Root;
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
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения ReadAppServerSettings возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult CheckSqlConnection(Session session)
		{
			session.Log("Выполнение CheckSqlConnection");

			try
			{
				var address = session[SqlServerAddress];

				int port;
				if (!Int32.TryParse(session[SqlServerPort], out port))
					port = SqlServerPortDefault;

				var instanceName = session[SqlServerInstanceName];
				var dbIntegratedSecurity = session[SqlServerAuthenticationMode] == "0";
				var login = session[SqlServerLogin];
				var password = session[SqlServerPassword];

				var msg = String.Format(@"Соединение с сервером {0}\{1},{2}", address, instanceName, port);
				string errors;
				if (!CheckSqlServerConnection(address, port, instanceName, dbIntegratedSecurity, login, password, out errors))
				{
					msg = String.Format("{0} установить не удалось по причине ошибки: \n\n{1}", msg, errors);
					MessageBox.Show(msg, String.Format("Установка {0}", session[ProductName]), MessageBoxButton.OK, MessageBoxImage.Warning);
					return ActionResult.Success;
				}
				msg = String.Format("{0} успешно установлено", msg);
				MessageBox.Show(msg, String.Format("Установка {0}", session[ProductName]), MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения CheckSqlConnection возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult WriteAppServerSettings(Session session)
		{
			session.Log("Выполнение WriteAppServerSettings");

			try
			{
				session.Log("Каталог хранения конфигурации: [{0}]", ConfigDir);

				var doc = ReadConfigFile(session, Path.Combine(ConfigDir, ConfigFile));
				var root = (doc == null) ? null : doc.Root;
				if (root == null)
				{
					return ActionResult.Success;
				}

				SetElementValue(root, DBServerAddressNodeName, session[SqlServerAddress]);
				SetElementValue(root, DBServerPortNodeName, session[SqlServerPort]);
				SetElementValue(root, DBServerNameNodeName, session[SqlServerInstanceName]);
				SetElementValue(root, DBUseIntegratedSecurityNodeName, session[SqlServerAuthenticationMode] == "0");
				SetElementValue(root, DBUserIDNodeName, session[SqlServerLogin]);
				SetElementValue(root, DBUserPwdNodeName, session[SqlServerPassword]);

				WriteConfigFile(doc, Path.Combine(ConfigDir, ConfigFile));
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения WriteAppServerSettings возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		private static XDocument ReadConfigFile(Session session, string filePath)
		{
			var dir = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(dir))
			{
				session.Log("Каталог хранения конфигурации [{0}] не найден", dir);
				return null;
			}

			if (!File.Exists(filePath))
			{
				session.Log("Конфигурационный файл [{0}] не найден", filePath);
				return null;
			}

			XDocument doc = null;
			using (var reader = new StreamReader(filePath))
			{
				doc = XDocument.Load(reader);
			}

			return doc;
		}

		private static string GetElementValue(XElement rootElement, string elementName)
		{
			var element = rootElement.Elements().FirstOrDefault(e => e.Name == elementName);
			return (element == null) ? null : element.Value;
		}

		private static string GetSqlServerAddress(XElement root)
		{
			return GetElementValue(root, DBServerAddressNodeName) ?? SqlServerAddressDefault;
		}

		private static int GetSqlServerPort(XElement root)
		{
			var result = GetElementValue(root, DBServerPortNodeName);
			if (result == null)
				return SqlServerPortDefault;

			int resultInt;
			return int.TryParse(result, out resultInt) ? resultInt : SqlServerPortDefault;
		}

		private static string GetSqlServerInstanceName(XElement root)
		{
			return GetElementValue(root, DBServerNameNodeName) ?? SqlServerInstanceNameDefault;
		}

		private static bool GetSqlServerAuthenticationMode(XElement root)
		{
			var result = GetElementValue(root, DBUseIntegratedSecurityNodeName);
			if (result == null)
				return SqlServerAuthenticationModeDefault;

			bool resultBool;
			return Boolean.TryParse(result, out resultBool) ? resultBool : SqlServerAuthenticationModeDefault;
		}

		private static string GetSqlServerLogin(XElement root)
		{
			return GetElementValue(root, DBUserIDNodeName) ?? SqlServerLoginDefault;
		}

		private static string GetSqlServerPassword(XElement root)
		{
			return GetElementValue(root, DBUserPwdNodeName) ?? SqlServerPasswordDefault;
		}

		/// <summary>
		/// Проверяет доступность СУБД MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название именованной установки сервера СУБД</param>
		/// <param name="useIntegratedSecurity">Метод аутентификации</param>
		/// <param name="userID">Логин (только для SQL Server аутентификации)</param>
		/// <param name="userPwd">Пароль (только для SQL Server аутентификации)</param>
		/// <param name="errors">Ошибки, возникшие в процессе проверки соединения</param>
		/// <returns>true - в случае успеха, false - в противном случае</returns>
		private static bool CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID, string userPwd, out string errors)
		{
			errors = null;
			var connectionString = BuildConnectionString(ipAddress, ipPort, instanceName, "master", useIntegratedSecurity, userID, userPwd);
			using (var connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch (Exception e)
				{
					errors = e.Message;
					return false;
				}
			}
			return true;
		}

		private static string BuildConnectionString(string ipAddress, int ipPort, string instanceName, string db, bool useIntegratedSecurity, string userID, string userPwd)
		{
			var csb = new SqlConnectionStringBuilder();
			csb.DataSource = String.Format(@"{0}{1},{2}", ipAddress, String.IsNullOrEmpty(instanceName) ? String.Empty : String.Format(@"\{0}", instanceName), ipPort);
			csb.InitialCatalog = db;
			csb.IntegratedSecurity = useIntegratedSecurity;
			if (!csb.IntegratedSecurity)
			{
				csb.UserID = userID;
				csb.Password = userPwd;
			}
			return csb.ConnectionString;
		}

		/// <summary>
		/// Сохраняет конфигурацию приложения в файл
		/// </summary>
		/// <param name="document">Конфигурация приложения</param>
		/// <param name="filePath">Путь к сохраняемому файлу</param>
		private static void WriteConfigFile(XDocument document, string filePath)
		{
			using (var stream = new StreamWriter(filePath, false))
			{
				document.Save(stream);
			}
		}

		private static void SetElementValue(XElement root, string elementName, object value)
		{
			var element = root.Elements().FirstOrDefault(e => e.Name == elementName);
			if (element == null)
				return;
			element.SetValue(value);
		}

		#endregion </Работа с файлом конфигурации AppServerSettings.xml>

		#region <Работа с файлами конфигурации Strazh*.exe.config>

		//Переменные сессии
		private const string Culture = "CULTURE";
		private const string TargetDir = "TARGETDIR";
		private const string InstallDir = "INSTALLLOCATION";
		private const string AdminDir = "ADMINISTRATORLOCATION";
		private const string MonitorDir = "MONITORLOCATION";
		private const string ServerDir = "SERVERLOCATION";
		private const string XDocCultureKey = "DefaultCulture";

		/// <summary>
		/// CustomAction для перезаписи конфига на культуру
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		[CustomAction]
		public static ActionResult WriteAppLocalizationSettings(Session session)
		{
			session.Log("Выполнение WriteAppLocalizationSettings");

			try
			{
				UpdateCultureInConfigFile(session, new InstalledAdministratorProperties(session));
				UpdateCultureInConfigFile(session, new InstalledMonitorProperties(session));
				UpdateCultureInConfigFile(session, new InstalledMonitorLayoutProperties(session));
				UpdateCultureInConfigFile(session, new InstalledServiceProperties(session));
				UpdateCultureInConfigFile(session, new InstalledServiceMonitorProperties(session));
			}
			catch (Exception e)
			{
				session.Log("В результате выполнения WriteAppLocalizationSettings возникла ошибка: {0}", e.Message);
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}
		
		/// <summary>
		/// В конфигурационном файле приложения прописывает культуру
		/// </summary>
		/// <param name="session">Сессия инсталлятора</param>
		/// <param name="applicationProperties">Параметры установленного приложения</param>
		private static void UpdateCultureInConfigFile(Session session, InstalledApplicationProperties applicationProperties)
		{
			var culture = session[Culture];
			var filePath = Path.Combine(applicationProperties.DestinationFolder, applicationProperties.AppConfigFileName);
			var xDoc = ReadConfigFile(session, filePath);
			if(xDoc == null)
				return;
			SetAppSettingsNodeChildElementValueByKey(xDoc, XDocCultureKey, culture);
			WriteConfigFile(xDoc, filePath);
		}

		/// <summary>
		/// В конфигурационном файле приложения в секции "appSettings" для дочернего элемента с заданным ключем
		/// устанавливает заданное значение
		/// </summary>
		/// <param name="doc">xml файл</param>
		/// <param name="key">Ключ, по которому ищем поле</param>
		/// <param name="value">Устанавливаемое для поля значение</param>
		private static void SetAppSettingsNodeChildElementValueByKey(XDocument doc, string key, object value)
		{
			var list = from appNode in doc.Descendants("appSettings").Elements()
					   where appNode.Attribute("key").Value == key
					   select appNode;
			var element = list.FirstOrDefault();
			if (element != null)
				element.Attribute("value").SetValue(value);
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения
		/// </summary>
		private abstract class InstalledApplicationProperties
		{
			#region Свойства и поля

			/// <summary>
			/// Каталог устаовки
			/// </summary>
			public string DestinationFolder { get; protected set; }

			/// <summary>
			/// Название конфигурационного файла
			/// </summary>
			public string AppConfigFileName { get; protected set; }

			#endregion

			#region Конструктор

			protected InstalledApplicationProperties(Session session)
			{
				DestinationFolder = Path.Combine(session[TargetDir], session[InstallDir]);
			}

			#endregion
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения "Сервер"
		/// </summary>
		private class InstalledServiceProperties : InstalledApplicationProperties
		{
			#region Конструктор

			public InstalledServiceProperties(Session session)
				: base(session)
			{
				DestinationFolder = Path.Combine(DestinationFolder, session[ServerDir]);
				AppConfigFileName = "StrazhService.exe.config";
			}

			#endregion
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения "Монитор сервера"
		/// </summary>
		private class InstalledServiceMonitorProperties : InstalledApplicationProperties
		{
			#region Конструктор

			public InstalledServiceMonitorProperties(Session session)
				: base(session)
			{
				DestinationFolder = Path.Combine(DestinationFolder, session[ServerDir]);
				AppConfigFileName = "StrazhService.Monitor.exe.config";
			}

			#endregion
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения "Администратор"
		/// </summary>
		private class InstalledAdministratorProperties : InstalledApplicationProperties
		{
			#region Конструктор

			public InstalledAdministratorProperties(Session session)
				: base(session)
			{
				DestinationFolder = Path.Combine(DestinationFolder, session[AdminDir]);
				AppConfigFileName = "StrazhAdmin.exe.config";
			}

			#endregion
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения "ОЗ"
		/// </summary>
		private class InstalledMonitorProperties : InstalledApplicationProperties
		{
			#region Конструктор

			public InstalledMonitorProperties(Session session)
				: base(session)
			{
				DestinationFolder = Path.Combine(DestinationFolder, session[MonitorDir]);
				AppConfigFileName = "StrazhMonitor.exe.config";
			}

			#endregion
		}

		/// <summary>
		/// Класс, описывающий параметры установленного приложения "ОЗ с макетами"
		/// </summary>
		private class InstalledMonitorLayoutProperties : InstalledApplicationProperties
		{
			#region Конструктор

			public InstalledMonitorLayoutProperties(Session session)
				: base(session)
			{
				DestinationFolder = Path.Combine(DestinationFolder, session[MonitorDir]);
				AppConfigFileName = "StrazhMonitor.Layout.exe.config";
			}

			#endregion
		}

		#endregion
	}
}