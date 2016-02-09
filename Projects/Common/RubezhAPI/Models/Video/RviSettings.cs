using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			Ip = "172.16.5.7";
			Port = 8000;
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
		[DataMember]
		public int VideoWidth { get; set; }
		[DataMember]
		public int VideoHeight { get; set; }
		[DataMember]
		public int VideoMarginLeft { get; set; }
		[DataMember]
		public int VideoMarginTop { get; set; }
	}
}
