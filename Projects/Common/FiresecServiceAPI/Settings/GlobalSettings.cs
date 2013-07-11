using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			FSAgent_UseFS2 = false;
			RemoteAddress = "localhost";
			RemotePort = 8000;
			Login = "adm";
			Password = "";
			AutoConnect = true;
			DoNotOverrideFS1 = false;
			LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
			Server_EnableRemoteConnections = false;
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

			Monitor_F1_Enabled = false;
			Monitor_F2_Enabled = true;
			Monitor_F3_Enabled = true;
			Monitor_F4_Enabled = true;
			Monitor_HaspInfo_Enabled = false;

			Administrator_HideMainMenuIcons = false;
			Administrator_IsExpertMode = false;
			Administrator_ShowMainMenu = false;
			Administrator_ShowGridLineInstruments = false;
			Administrator_ShowDirectories = false;
			Administrator_ShowSimulation = false;
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
		public bool DoNotAutoconnectAdm { get; set; }

		[DataMember]
		public bool DoNotOverrideFS1 { get; set; }

		[DataMember]
		public string FS_RemoteAddress { get; set; }

		[DataMember]
		public int FS_Port { get; set; }

		[DataMember]
		public string FS_Login { get; set; }

		[DataMember]
		public string FS_Password { get; set; }

		[DataMember]
		public bool Server_EnableRemoteConnections { get; set; }

		[DataMember]
		public bool IsImitatorEnabled { get; set; }

		[DataMember]
		public string LibVlcDllsPath { get; set; }

		[DataMember]
		public bool ForceShutdown { get; set; }
		[DataMember]
		public bool FSAgent_UseFS2 { get; set; }

		[DataMember]
		public string Modules { get; set; }

		[DataMember]
		public bool Monitor_F1_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F2_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F3_Enabled { get; set; }
		[DataMember]
		public bool Monitor_F4_Enabled { get; set; }
		[DataMember]
		public bool Monitor_HaspInfo_Enabled { get; set; }
		[DataMember]
		public bool Monitor_DoNotShowConfirmatinoOnIgnore { get; set; }
		[DataMember]
		public bool Monitor_DoNotShowZones { get; set; }
		[DataMember]
		public bool Monitor_HidePlansTree { get; set; }

		[DataMember]
		public bool Administrator_IsExpertMode { get; set; }
		[DataMember]
		public bool Administrator_IsMenuIconText { get; set; }
		[DataMember]
		public bool Administrator_HideMainMenuIcons { get; set; }
		[DataMember]
		public bool Administrator_ShowMainMenu { get; set; }
		[DataMember]
		public bool Administrator_HidePlanAlignInstruments { get; set; }
		[DataMember]
		public bool Administrator_ShowGridLineInstruments { get; set; }
		[DataMember]
		public bool Administrator_ShowDirectories { get; set; }
		[DataMember]
		public bool Administrator_ShowSimulation { get; set; }

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