using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class EmailSettings
	{
		public EmailSettings()
		{
		}

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public string Port { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string Password { get; set; }

		public static EmailSettings SetDefaultParams()
		{
			EmailSettings defaultParams = new EmailSettings();
			defaultParams.Ip = "mail.rubezh.ru";
			defaultParams.Port = ((int)25).ToString();
			defaultParams.UserName = "obychevma@rubezh.ru";
			defaultParams.Password = "Aiciir5kee";
			return defaultParams;
		}
	}
}