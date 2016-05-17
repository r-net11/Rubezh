using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI
{
	[DataContract]
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			RemoteAddress = "localhost";
			RemotePort = 8000;
			ReportRemotePort = 8800;
			Login = "adm";
			Password = "";
			AutoConnect = false;

			ModuleItems = new List<string>();

			Monitor_F1_Enabled = false;
			Monitor_F2_Enabled = true;
			Monitor_F3_Enabled = true;
			Monitor_F4_Enabled = true;
			Monitor_IsControlMPT = false;
			Monitor_HaspInfo_Enabled = false;
			IgnoredErrors = 0;
		}

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public int ReportRemotePort { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool AutoConnect { get; set; }

		[DataMember]
		public bool DoNotAutoconnectAdm { get; set; }

		[DataMember]
		public bool RunRevisor { get; set; }

		[DataMember]
		public List<string> ModuleItems { get; set; }

		[DataMember]
		public bool Monitor_F1_Enabled { get; set; }

		[DataMember]
		public bool Monitor_F2_Enabled { get; set; }

		[DataMember]
		public bool Monitor_F3_Enabled { get; set; }

		[DataMember]
		public bool Monitor_F4_Enabled { get; set; }

		[DataMember]
		public bool Monitor_IsControlMPT { get; set; }

		[DataMember]
		public bool Monitor_HaspInfo_Enabled { get; set; }

		[DataMember]
		public bool Monitor_HidePlansTree { get; set; }

		[DataMember]
		public ValidationErrorType IgnoredErrors { get; set; }

		public void SetDefaultModules()
		{
			ModuleItems = new List<string>
			{
				"PlansModule.dll",
				"SecurityModule.dll",
				"SKDModule.dll",
				"StrazhModule.dll",
				"VideoModule.dll",
				"AutomationModule.dll",
				"LayoutModule.dll",
				"ReportsModule.dll",
				"FiltersModule.dll",
				"JournalModule.dll",
				"SoundsModule.dll",
				"SettingsModule.dll",
				"Integration.OPC.dll"
			};
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