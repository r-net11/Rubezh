using Common;
using System;
using System.Linq;
using System.Net;

namespace Infrastructure.Common
{
	/// <summary>
	/// Класс описывает сетевые настройки Сервера приложений и Сервера отчетов
	/// </summary>
	public static class ConnectionSettingsManager
	{
		private static string _remoteAddress;

		/// <summary>
		/// IP-адрес Сервера приложений
		/// </summary>
		public static string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				if (_remoteAddress == "localhost")
					_remoteAddress = "127.0.0.1";
			}
		}

		/// <summary>
		/// Порт Сервера приложений
		/// </summary>
		public static int RemotePort { get; set; }

		/// <summary>
		/// Порт Сервера отчетов
		/// </summary>
		public static int ReportRemotePort { get; set; }

		static ConnectionSettingsManager()
		{
			try
			{
				RemoteAddress = AppServerSettingsHelper.AppServerSettings.ServiceAddress;
				RemotePort = AppServerSettingsHelper.AppServerSettings.ServicePort;
				ReportRemotePort = AppServerSettingsHelper.AppServerSettings.ReportServicePort;
			}
			catch (Exception e)
			{
				Logger.Error(e, "AppSettingsManager.AppSettingsManager");
			}
		}

		/// <summary>
		/// URI Сервера приложений
		/// </summary>
		public static string ServerAddress
		{
			get
			{
				var serviceAddress = "net.pipe://127.0.0.1/FiresecService/";
				if (IsRemote)
				{
					serviceAddress = "net.tcp://" + RemoteAddress + ":" + RemotePort.ToString() + "/FiresecService/";
				}
				return serviceAddress;
			}
		}

		/// <summary>
		/// URI Сервера отчетов
		/// </summary>
		public static string ReportServerAddress
		{
			get
			{
				var serviceAddress = "net.tcp://" + GetIPAddress() + ":" + ReportRemotePort.ToString() + "/ReportFiresecService/";
				return serviceAddress;
			}
		}

		/// <summary>
		/// Определяет доступность Сервера приложений извне
		/// </summary>
		public static bool IsRemote
		{
			get
			{
				if (string.IsNullOrEmpty(RemoteAddress))
					return false;
				return (RemoteAddress != "localhost" && RemoteAddress != "127.0.0.1");
			}
		}

		/// <summary>
		/// Выбирает IP-адрес для запуска WCF-Сервиса
		/// </summary>
		/// <returns>IP-адрес</returns>
		public static string GetIPAddress()
		{
			// Получаем список IP-адресов хоста
			var hostIpAdresses = NetworkHelper.GetHostIpAddresses();

			// Получаем IP-адрес из конфигурации
			var ipAddressFromConfig = AppServerSettingsHelper.AppServerSettings.ServiceAddress;

			// Если IP-адрес из конфигурации входит в список IP-адресов хоста, то используем его
			if (hostIpAdresses.Any(x => x == ipAddressFromConfig))
				return ipAddressFromConfig;

			// В противном случае, используем "localhost"
			return "localhost";
		}
	}
}