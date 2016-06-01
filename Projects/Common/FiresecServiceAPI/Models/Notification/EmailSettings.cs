using System.Globalization;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class EmailSettings
	{
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
			var defaultParams = new EmailSettings
			{
				Ip = "mail.rubezh.ru",
				Port = 25.ToString(CultureInfo.InvariantCulture),
				UserName = "obychevma@rubezh.ru",
				Password = "Aiciir5kee"
			};
			return defaultParams;
		}
	}
}