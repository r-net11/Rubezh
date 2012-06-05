using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ClientCredentials
	{
		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public ClientType ClientType { get; set; }

		[DataMember]
		public Guid ClientUID { get; set; }

		[DataMember]
		public string ClientCallbackAddress { get; set; }
	}
}