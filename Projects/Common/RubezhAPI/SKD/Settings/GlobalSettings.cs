using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace RubezhAPI
{
	[DataContract]
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			RemoteAddress = "localhost";
			RemotePort = 8000;
			ReportRemotePort = 8800;
			AdminLogin = "adm";
			AdminPassword = "";
			AdminAutoConnect = false;
			MonitorLogin = "adm";
			MonitorPassword = "";
			WebLogin = "adm";
			WebPassword = "";
			MonitorAutoConnect = false;

			Server_EnableRemoteConnections = false;
			IgnoredErrors = new List<ValidationErrorType>();

			//DbConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=Rubezh;Integrated Security=True;Language='English'";
			DbSettings = new RubezhAPI.DbSettings
			{
				ConnectionString = @"Server=localhost;Database=Rubezh;User Id=asd;Password=1;",
				DbType = RubezhAPI.DbType.Postgres,
				IsFullConnectionString = true
			};
			ModuleItems = new List<string>();

			Monitor_F1_Enabled = false;
			Monitor_F2_Enabled = true;
			Monitor_F3_Enabled = true;
			Monitor_F4_Enabled = true;
		}

		[DataMember]
		public string AdminLogin { get; set; }

		[DataMember]
		public string AdminPassword { get; set; }

		[DataMember]
		public bool AdminAutoConnect { get; set; }

		[DataMember]
		public string MonitorLogin { get; set; }

		[DataMember]
		public string MonitorPassword { get; set; }
		[DataMember]
		public string WebLogin { get; set; }

		[DataMember]
		public string WebPassword { get; set; }

		[DataMember]
		public bool MonitorAutoConnect { get; set; }

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public int ReportRemotePort { get; set; }

		[DataMember]
		public bool RunRevisor { get; set; }

		[DataMember]
		public bool Server_EnableRemoteConnections { get; set; }

		[DataMember]
		public string Server_RemoteIpAddress { get; set; }

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
		public List<ValidationErrorType> IgnoredErrors { get; set; }

		[DataMember]
		public bool ShowZonesDevices { get; set; }

		[DataMember]
		public bool ShowDoorsDevices { get; set; }

		[DataMember]
		public bool ShowMPTsDevices { get; set; }

		[DataMember]
		public bool ShowGuardZonesDevices { get; set; }

		[DataMember]
		public bool ShowDoorsDevicesForZone { get; set; }

		[DataMember]
		public bool ShowMPTsDevicesForZone { get; set; }

		[DataMember]
		public DbSettings DbSettings { get; set; }

		public void SetDefaultModules()
		{
			ModuleItems = new List<string>();

			ModuleItems.Add("PlansModule.dll");
			//ModuleItems.Add("PlansModule.Kursk.dll");
			ModuleItems.Add("SecurityModule.dll");
			ModuleItems.Add("GKModule.dll");
			ModuleItems.Add("SKDModule.dll");
			ModuleItems.Add("VideoModule.dll");
			ModuleItems.Add("AutomationModule.dll");
			ModuleItems.Add("LayoutModule.dll");
			ModuleItems.Add("ReportsModule.dll");
			ModuleItems.Add("FiltersModule.dll");
			ModuleItems.Add("JournalModule.dll");
			ModuleItems.Add("SoundsModule.dll");
			ModuleItems.Add("SettingsModule.dll");
			//ModuleItems.Add("DiagnosticsModule.dll");
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}
	}
}