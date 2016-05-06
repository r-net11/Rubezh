using System;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
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

		public string FriendlyUserName { get; set; }

		public string ClientIpAddress { get; set; }

		public string ClientIpAddressAndPort { get; set; }

		public string UniqueId
		{
			get
			{
				return ClientType.ToString() + "." + (ClientIpAddress == null ? "" : ClientIpAddress);
			}
		}
	}
}