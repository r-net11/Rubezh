using Common;
using System;
using System.Linq;
using System.Net;

namespace Infrastructure.Common
{
	/// <summary>
	/// Класс описывает клиентские сетевые настройки для соединения с Сервером приложений и Сервером отчетов
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
				if (_remoteAddress == NetworkHelper.Localhost)
					_remoteAddress = NetworkHelper.LocalhostIp;
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
			Update();
		}

		public static void Update()
		{
			try
			{
				RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
				RemotePort = GlobalSettingsHelper.GlobalSettings.RemotePort;
				ReportRemotePort = GlobalSettingsHelper.GlobalSettings.ReportRemotePort;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConnectionSettingsManager.Update");
			}
		}

		/// <summary>
		/// URI Сервера приложений
		/// </summary>
		public static string ServerAddress
		{
			get
			{
				var serviceAddress = String.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerConstants.ServiceName);
				if (IsRemote)
				{
					serviceAddress = String.Format("net.tcp://{0}:{1}/{2}/", RemoteAddress, RemotePort, AppServerConstants.ServiceName);
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
				return IsRemote
					? String.Format("net.tcp://{0}:{1}/{2}/", RemoteAddress, ReportRemotePort, AppServerConstants.ReportServiceName)
					: String.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerConstants.ReportServiceName);
				//return String.Format("net.tcp://{0}:{1}/{2}/", RemoteAddress, ReportRemotePort, AppServerServices.ReportServiceName);
			}
		}

		/// <summary>
		/// Определяет расположение Сервера приложений (локально/удаленно)
		/// </summary>
		public static bool IsRemote
		{
			get
			{
				if (string.IsNullOrEmpty(RemoteAddress))
					return false;
				return !NetworkHelper.IsLocalAddress(RemoteAddress);
			}
		}
	}
}