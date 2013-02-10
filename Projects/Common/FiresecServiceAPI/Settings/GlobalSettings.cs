using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			RemoteAddress = "localhost";
			RemotePort = 8000;
			Login = "adm";
			Password = "";
			AutoConnect = true;
			DoNotOverrideFS1 = false;
			LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
			IsExpertMode = false;
			EnableRemoteConnections = true;
			IsImitatorEnabled = false;
			Modules =
				"DevicesModule.dll" + "\r\n" +
				"PlansModule.dll" + "\r\n" +
				"LibraryModule.dll" + "\r\n" +
				"SecurityModule.dll" + "\r\n" +
				"FiltersModule.dll" + "\r\n" +
				"SoundsModule.dll" + "\r\n" +
				"InstructionsModule.dll" + "\r\n" +
				"SettingsModule.dll" + "\r\n" +
				"GKModule.dll" + "\r\n" +
				"OPCModule.dll" + "\r\n" +
				"NotificationModule.dll" + "\r\n" +
				"VideoModule.dll" + "\r\n" +
				"DiagnosticsModule.dll" + "\r\n" +
				"AlarmModule.dll" + "\r\n" +
				"JournalModule.dll" + "\r\n" +
				"ReportsModule.dll";
			FS_RemoteAddress = "localhost";
			FS_Port = 211;
			FS_Login = "adm";
			FS_Password = "";
		}

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool AutoConnect { get; set; }

		[DataMember]
		public bool DoNotOverrideFS1 { get; set; }

		[DataMember]
		public string LibVlcDllsPath { get; set; }

		[DataMember]
		public bool IsExpertMode { get; set; }

		[DataMember]
		public bool EnableRemoteConnections { get; set; }

		[DataMember]
		public bool IsImitatorEnabled { get; set; }

		[DataMember]
		public string Modules { get; set; }

		[DataMember]
		public string FS_RemoteAddress { get; set; }

		[DataMember]
		public int FS_Port { get; set; }

		[DataMember]
		public string FS_Login { get; set; }

		[DataMember]
		public string FS_Password { get; set; }

		public List<string> GetModules()
		{

			var modules = Modules.Replace("\r\n", ";").Split(';');
			return modules.ToList();
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}
	}
}