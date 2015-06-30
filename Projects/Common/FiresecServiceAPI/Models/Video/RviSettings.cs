using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			Ip = "172.16.5.7";
			Port = 8000;
			Login = "strazh";
			Password = "strazh12345";
			DllsPath = @"..\VLC\";
			PluginsPath = @"..\VLC\plugins\";
#if DEBUG
			DllsPath = @"..\..\..\..\3rdParty\VLC\";
			PluginsPath = @"..\..\..\..\3rdParty\VLC\plugins\";
#endif
		}

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string DllsPath { get; set; }

		[DataMember]
		public string PluginsPath { get; set; }
	}
}
