using System;
using System.Configuration;
using Common;

namespace Infrastructure.Common
{
	public static class AppSettingsManager
	{
		public static string RemoteAddress { get; set; }

		static AppSettingsManager()
		{
			try
			{
				RemoteAddress = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
				if (RemoteAddress == "localhost")
					RemoteAddress = "127.0.0.1";
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
					serviceAddress = "http://" + RemoteAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FiresecService/";
				}
				return serviceAddress;
			}
		}

		public static string FSAgentServerAddress
		{
			get
			{
				var serviceAddress = "net.pipe://127.0.0.1/FSAgent/";
				if (IsRemote)
				{
					serviceAddress = "http://" + RemoteAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FSAgent/";
				}
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
	}
}