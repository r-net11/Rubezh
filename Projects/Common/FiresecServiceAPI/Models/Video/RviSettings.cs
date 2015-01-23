using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			Ip = "192.168.0.1";
			Port = 37777;
			Login = "admin";
			Password = "admin";
		}

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }
	}
}
