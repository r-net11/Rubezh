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
			RemoteFSAgentPort = 8001;
			AutoConnect = true;
			DoNotOverrideFS1 = false;
			LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
		}

		[DataMember]
		public string RemoteAddress { get; set; }

		[DataMember]
		public int RemotePort { get; set; }

		[DataMember]
		public int RemoteFSAgentPort { get; set; }

		[DataMember]
		public bool AutoConnect { get; set; }

		[DataMember]
		public bool DoNotOverrideFS1 { get; set; }

		[DataMember]
		public string LibVlcDllsPath { get; set; }

		[DataMember]
		public string Modules { get; set; }

		public List<string> GetModules()
		{
			var modules = Modules.Split('\n');
			return modules.ToList();
		}
	}
}