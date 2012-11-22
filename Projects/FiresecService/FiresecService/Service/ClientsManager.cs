using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	public static class ClientsManager
	{
		public static List<ClientInfo> ClientInfos = new List<ClientInfo>();

		public static void Add(Guid uid, ClientCredentials clientCredentials)
		{
			if (ClientInfos.Any(x => x.UID == uid))
				return;

			var clientInfo = new ClientInfo();
			clientInfo.UID = uid;
			clientInfo.ClientCredentials = clientCredentials;
			ClientInfos.Add(clientInfo);

			MainViewModel.Current.AddClient(clientCredentials);
		}

		public static void Remove(Guid uid)
		{
			var clientInfo = ClientInfos.FirstOrDefault(x=>x.UID == uid);
			ClientInfos.Remove(clientInfo);
			MainViewModel.Current.RemoveClient(uid);
		}

		public static ClientCredentials Get(Guid uid)
		{
			var clientInfo = ClientInfos.FirstOrDefault(x=>x.UID == uid);
			if (clientInfo != null)
			{
				return clientInfo.ClientCredentials;
			}
			return null;
		}
	}

	public class ClientInfo
	{
		public Guid UID { get; set; }
		public ClientCredentials ClientCredentials { get; set; }
		public int CallbackIndex { get; set; }
	}
}