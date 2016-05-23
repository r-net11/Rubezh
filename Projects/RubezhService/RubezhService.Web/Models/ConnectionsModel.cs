using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

namespace RubezhService.Models
{
	static class ConnectionsModel
	{
		public static List<Connection> Connections { get; private set; }
		static ConnectionsModel()
		{
			Connections = new List<Connection>();
		}
		public static void AddConnection(ClientCredentials clientCredentials)
		{
			Connections.Add(new Connection(clientCredentials));
            // TODO: Notify
        }

        public static void RemoveConnection(Guid uid)
		{
			Connections.RemoveAll(x => x.UID == uid);
            // TODO: Notify
        }
    }

	public class Connection
	{
		public Guid UID { get; private set; }
		public string ClientType { get; private set; }
		public string IpAddress { get; private set; }
		public string FriendlyUserName { get; private set; }

		public Connection(ClientCredentials clientCredentials)
		{
			ClientType = clientCredentials.ClientType.ToDescription();
			UID = clientCredentials.ClientUID;
			FriendlyUserName = clientCredentials.FriendlyUserName;
			IpAddress = clientCredentials.ClientIpAddress;
			if (IpAddress.StartsWith("127.0.0.1"))
				IpAddress = "localhost";
		}


	}
}
