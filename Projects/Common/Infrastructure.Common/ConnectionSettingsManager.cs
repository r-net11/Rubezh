using System;
using System.Linq;
using System.Net;
using Common;

namespace Infrastructure.Common
{
	public static class ConnectionSettingsManager
	{
		
		public static string RemoteAddress
		{
			get
			{
				if (GlobalSettingsHelper.GlobalSettings.RemoteAddress == "localhost")
					return "127.0.0.1";
				return GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			}

		}
		
		public static string ServerAddress
		{
			get
			{
				var serviceAddress = "net.pipe://127.0.0.1/FiresecService/";
				if (IsRemote)
				{
					serviceAddress = "net.tcp://" + RemoteAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort + "/FiresecService/";
				}
				return serviceAddress;
			}
		}

		public static string ReportServerAddress
		{
			get
			{
				var serviceAddress = "net.tcp://" + GetIPAddress() + ":" + GlobalSettingsHelper.GlobalSettings.ReportRemotePort + "/ReportFiresecService/";
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

		public static string GetIPAddress()
		{
			try
			{
				var hostName = System.Net.Dns.GetHostName();
				IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName);
				IPAddress[] addresses = ipEntry.AddressList;
				var ipV6Address = addresses.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
				return ipV6Address.ToString();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.GetIPAddress");
				return "localhost";
			}
		}
	}
}