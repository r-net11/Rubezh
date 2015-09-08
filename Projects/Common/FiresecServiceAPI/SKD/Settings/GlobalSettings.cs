using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace FiresecAPI
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
			MonitorAutoConnect = false;

			Server_EnableRemoteConnections = false;

			//DbConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=Rubezh;Integrated Security=True;Language='English'";
			DbConnectionString = @"Server=localhost;Database=Rubezh;User Id=asd;Password=1;";
			DbType = FiresecAPI.DbType.Postgres;
			
			ModuleItems = new List<string>();

			Monitor_F1_Enabled = false;
			Monitor_F2_Enabled = true;
			Monitor_F3_Enabled = true;
			Monitor_F4_Enabled = true;
			IgnoredErrors = 0;
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
		public bool MonitorAutoConnect { get; set; }

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public int ReportRemotePort { get; set; }

		[DataMember]
		public bool DoNotAutoconnectAdm { get; set; }

		[DataMember]
		public bool RunRevisor { get; set; }

		[DataMember]
		public bool Server_EnableRemoteConnections { get; set; }

		[DataMember]
		public string DbConnectionString { get; set; }

		[DataMember]
		public DbType DbType { get; set; }

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
		public bool Monitor_HidePlansTree { get; set; }

		[DataMember]
		public ValidationErrorType IgnoredErrors { get; set; }

		[DataMember]
		public bool ShowOtherZonesDevices { get; set; }

		[DataMember]
		public bool ShowDoorsDevices { get; set; }

		[DataMember]
		public bool ShowMPTsDevices { get; set; }

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