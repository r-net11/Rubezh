﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AppServerSettings
	{
		public AppServerSettings()
		{
			ServiceAddress = "localhost";
			ServicePort = 8000;
			ReportServicePort = 8800;
			EnableRemoteConnections = false;
			UseHasp = false;
			CreateNewDBOnOversize = true;
			EnableOfflineLog = true;

			DBServerAddress = ".";
			DBServerPort = 1433;
			DBServerName = "SQLEXPRESS";
			DBUseIntegratedSecurity = true;
			DBUserID = String.Empty;
			DBUserPwd = String.Empty;
		}

		[DataMember]
		public string ServiceAddress { get; set; }

		[DataMember]
		public int ServicePort { get; set; }

		[DataMember]
		public int ReportServicePort { get; set; }

		[DataMember]
		public bool EnableRemoteConnections { get; set; }

		[DataMember]
		public bool UseHasp { get; set; }

		[DataMember]
		public string DBServerAddress { get; set; }

		[DataMember]
		public int DBServerPort { get; set; }

		[DataMember]
		public string DBServerName { get; set; }

		[DataMember]
		public bool DBUseIntegratedSecurity { get; set; }

		[DataMember]
		public string DBUserID { get; set; }

		[DataMember]
		public string DBUserPwd { get; set; }

		[DataMember]
		public bool CreateNewDBOnOversize { get; set; }
	}
}