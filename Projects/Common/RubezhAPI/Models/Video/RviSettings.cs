using System.Runtime.Serialization;

namespace RubezhAPI.Models
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
		[DataMember]
		public int VideoWidth { get; set; }
		[DataMember]
		public int VideoHeight { get; set; }
		[DataMember]
		public int VideoMarginLeft { get; set; }
		[DataMember]
		public int VideoMarginTop { get; set; }
		public string Url
		{
			get
			{
				return string.Format("net.tcp://{0}:{1}/Integration", Ip, Port);
			}
		}
	}
}