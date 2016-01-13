using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ClientCredentials
	{
		[DataMember]
		public string Login { get; set; }

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

		public bool IsRemote
		{
			get
			{
				if (string.IsNullOrEmpty(ClientIpAddress))
					return false;
				return (ClientIpAddress != "localhost" && ClientIpAddress != "127.0.0.1");
			}
		}
	}
}