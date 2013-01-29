using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class SenderParams
	{
		public SenderParams()
		{
			;
		}

		[DataMember]
		public string From { get; set; }

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public string Port { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string Password { get; set; }

		public static SenderParams SetDefaultParams()
		{
			SenderParams defaultParams = new SenderParams();
			defaultParams.From = "obychevma@rubezh.ru";
			defaultParams.Ip = "mail.rubezh.ru";
			defaultParams.Port = ((int)25).ToString();
			defaultParams.UserName = "obychevma@rubezh.ru";
			defaultParams.Password = "Aiciir5kee";
			return defaultParams;
		}
	}
}