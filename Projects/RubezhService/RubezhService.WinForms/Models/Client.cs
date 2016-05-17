using RubezhAPI;
using RubezhAPI.Models;
using System;

namespace RubezhService.Models
{
	public class Client
	{
		public ClientCredentials ClientCredentials { get; private set; }
		public string ClientType { get; private set; }
		public Guid UID { get; private set; }
		public string IpAddress { get; private set; }

		public Client(ClientCredentials clientCredentials)
		{
			ClientCredentials = clientCredentials;
			ClientType = clientCredentials.ClientType.ToDescription();
			UID = ClientCredentials.ClientUID;
			FriendlyUserName = clientCredentials.FriendlyUserName;
			IpAddress = clientCredentials.ClientIpAddressAndPort;
			if (IpAddress.StartsWith("127.0.0.1"))
				IpAddress = "localhost";
		}

		public string FriendlyUserName { get; set; }
	}
}