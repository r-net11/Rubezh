using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common;

namespace Infrastructure.Common
{
	public static class AppSettingsManager
	{
		public static string FS_Address { get; set; }
		public static int FS_Port { get; set; }
		public static string FS_Login { get; set; }
		public static string FS_Password { get; set; }

		public static string RemoteAddress { get; set; }
		public static int RemotePort { get; set; }
		public static int RemoteFSAgentPort { get; set; }
		public static bool AutoConnect { get; set; }

		public static string Login { get; set; }
		public static string Password { get; set; }

		static AppSettingsManager()
		{
			try
			{
				FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
				FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
				FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
				AutoConnect = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string);

				RemoteAddress = ConfigurationManager.AppSettings["RemoteAddress"] as string;
				RemotePort = Convert.ToInt32(ConfigurationManager.AppSettings["RemotePort"] as string);
				RemoteFSAgentPort = Convert.ToInt32(ConfigurationManager.AppSettings["RemoteFSAgentPort"] as string);

				FS_Address = RemoteAddress;
				if (RemoteAddress == "localhost")
					RemoteAddress = "127.0.0.1";

				Login = ConfigurationManager.AppSettings["Login"] as string;
				Password = ConfigurationManager.AppSettings["Password"] as string;
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
					serviceAddress = "http://" + RemoteAddress + ":" + RemotePort.ToString() + "/FiresecService/";
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
					serviceAddress = "http://" + RemoteAddress + ":" + RemotePort.ToString() + "/FSAgent/";
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