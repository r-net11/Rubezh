using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class EmailData
	{
		public EmailData()
		{
			Emails = new List<Email>();
			EmailSettings = new EmailSettings();
		}

		[DataMember]
		public List<Email> Emails { get; set; }

		[DataMember]
		public EmailSettings EmailSettings { get; set; }
	}
}