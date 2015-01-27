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
			DllsPath = @"C:\Program Files (x86)\VideoLAN\VLC\";
			PluginsPath = @"C:\Program Files (x86)\VideoLAN\VLC\plugins\";
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
