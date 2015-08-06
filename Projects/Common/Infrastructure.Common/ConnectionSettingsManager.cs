using Common;
using System;
using System.Linq;
using System.Net;

namespace Infrastructure.Common
{
	public static class ConnectionSettingsManager
	{
		private static string _remoteAddress;

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

		public static int RemotePort { get; set; }

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

		public static string ReportServerAddress
		{
			get
			{
				var serviceAddress = "net.tcp://" + GetIPAddress() + ":" + ReportRemotePort.ToString() + "/ReportFiresecService/";
				return serviceAddress;
			}
		}

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
		/// <returns></returns>
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