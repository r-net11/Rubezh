using Common;
using System;
using System.Linq;

namespace Infrastructure.Common
{
	/// <summary>
	/// Класс описывает сетевые настройки для Сервера приложений и Сервера отчетов
	/// </summary>
	public static class AppServerConnectionManager
	{
		private static string _serverAddress;

		/// <summary>
		/// IP-адрес Сервера приложений
		/// </summary>
		public static string ServerAddress
		{
			get { return _serverAddress; }
			set
			{
				_serverAddress = value;
				if (_serverAddress == NetworkHelper.Localhost)
					_serverAddress = NetworkHelper.LocalhostIp;
			}
		}

		/// <summary>
		/// Порт Сервера приложений
		/// </summary>
		public static int ServerPort { get; set; }

		/// <summary>
		/// Порт Сервера отчетов
		/// </summary>
		public static int ReportServerPort { get; set; }

		/// <summary>
		/// Порт сервиса для получения отчётных данных
		/// </summary>
		public static int ReportDataServicePort { get; set; }

		static AppServerConnectionManager()
		{
			Update();
		}

		public static void Update()
		{
			try
			{
				ServerAddress = GetIpAddress();
				ServerPort = AppServerSettingsHelper.AppServerSettings.ServicePort;
				ReportServerPort = AppServerSettingsHelper.AppServerSettings.ReportServicePort;
				ReportDataServicePort = AppServerSettingsHelper.AppServerSettings.ReportDataServicePort;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConnectionSettingsManager.Update");
			}
		}

		/// <summary>
		/// URI Сервера приложений для протокола NamedPipes
		/// </summary>
		public static string ServerNamedPipesUri
		{
			get
			{
				return String.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerConstants.ServiceName);
			}
		}

		/// <summary>
		/// URI Сервера приложений для протокола Http
		/// </summary>
		public static string ServerHttpUri
		{
			get
			{
				return String.Format("http://{0}:{1}/{2}/", ServerAddress, ServerPort, AppServerConstants.ServiceName);
			}
		}

		/// <summary>
		/// URI Сервера приложений для протокола Tcp
		/// </summary>
		public static string ServerTcpUri
		{
			get
			{
				return String.Format("net.tcp://{0}:{1}/{2}/", ServerAddress, ServerPort, AppServerConstants.ServiceName);
			}
		}

		/// <summary>
		/// URI Сервера отчетов для протокола NamedPipes
		/// </summary>
		public static string ReportServerNamedPipesUri
		{
			get
			{
				return String.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerConstants.ReportServiceName);
			}
		}

		/// <summary>
		/// URI Сервера отчетов для протокола Tcp
		/// </summary>
		public static string ReportServerTcpUri
		{
			get
			{
				return String.Format("net.tcp://{0}:{1}/{2}/", ServerAddress, ReportServerPort, AppServerConstants.ReportServiceName);
			}
		}

		/// <summary>
		/// URI сервиса получения отчётных данных NamedPipes
		/// </summary>
		public static string ReportDataServerNamedPipesUri
		{
			get
			{
				return string.Format("net.tcp://{0}:{1}/ReportDataService/", NetworkHelper.LocalhostIp, AppServerConstants.ReportDataServiceName);
			}
		}

		/// <summary>
		/// URI сервиса получения отчётных данных Tcp
		/// </summary>
		public static string ReportDataServiceTcpUri
		{
			get
			{
				return string.Format("net.tcp://{0}:{1}/{2}/", ServerAddress, ReportDataServicePort, AppServerConstants.ReportDataServiceName);
			}
		}

		///// <summary>
		///// URI Сервера отчетов
		///// </summary>
		//public static string ReportServerUri
		//{
		//	get
		//	{
		//		//return IsRemote
		//		//	? String.Format("net.tcp://{0}:{1}/{2}/", ServerAddress, ReportServerPort, AppServerServices.ReportServiceName)
		//		//	: String.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerServices.ReportServiceName);
		//		return String.Format("net.tcp://{0}:{1}/{2}/", ServerAddress, ReportServerPort, AppServerServices.ReportServiceName);
		//	}
		//}

		/// <summary>
		/// Определяет расположение Сервера приложений (локально/удаленно)
		/// </summary>
		public static bool IsRemote
		{
			get
			{
				if (string.IsNullOrEmpty(ServerAddress))
					return false;
				return !NetworkHelper.IsLocalAddress(ServerAddress);
			}
		}

		/// <summary>
		/// Выбирает IP-адрес для запуска WCF-Сервиса
		/// </summary>
		/// <returns>IP-адрес</returns>
		public static string GetIpAddress()
		{
			// Получаем список IP-адресов хоста
			var hostIpAdresses = NetworkHelper.GetHostIpAddresses();

			// Получаем IP-адрес из конфигурации
			var ipAddressFromConfig = AppServerSettingsHelper.AppServerSettings.ServiceAddress;

			// Если IP-адрес из конфигурации входит в список IP-адресов хоста, то используем его
			if (hostIpAdresses.Any(x => x == ipAddressFromConfig))
				return ipAddressFromConfig;

			// В противном случае, используем "localhost"
			return NetworkHelper.Localhost;
		}
	}
}