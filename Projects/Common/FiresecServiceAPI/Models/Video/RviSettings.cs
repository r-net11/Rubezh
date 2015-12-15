using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			Ip = "localhost";
			Port = 8000;
			Login = "strazh";
			Password = "strazh12345";
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