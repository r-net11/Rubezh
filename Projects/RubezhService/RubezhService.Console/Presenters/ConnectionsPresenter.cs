using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

namespace RubezhService
{
	static class ConnectionsPresenter
	{
		public static List<Connection> Connections { get; private set; }
		static ConnectionsPresenter()
		{
			Connections = new List<Connection>();
		}
		public static void AddConnection(ClientCredentials clientCredentials)
		{
			Connections.Add(new Connection(clientCredentials));
			PageController.OnPageChanged(Page.Connections);
		}

		public static void RemoveConnection(Guid uid)
		{
			Connections.RemoveAll(x => x.UID == uid);
			PageController.OnPageChanged(Page.Connections);
		}
	}

	class Connection
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
