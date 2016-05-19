using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RubezhService.Service
{
	public static class ClientsManager
	{
		public static List<ClientInfo> ClientInfos = new List<ClientInfo>();

		public static bool Add(ClientCredentials clientCredentials)
		{
			if (ClientInfos.Any(x => x.UID == clientCredentials.ClientUID))
				return false;

			var result = true;
			var existingClientInfo = ClientInfos.FirstOrDefault(x => x.ClientCredentials.UniqueId == clientCredentials.UniqueId);
			if (existingClientInfo != null)
			{
				Remove(existingClientInfo.UID);
				result = false;
			}

			var clientInfo = new ClientInfo();
			clientInfo.UID = clientCredentials.ClientUID;
			clientInfo.ClientCredentials = clientCredentials;
			clientInfo.CallbackIndex = CallbackManager.Index;
			ClientInfos.Add(clientInfo);

			Notifier.AddClient(clientCredentials);
			return result;
		}

		public static void Remove(Guid uid)
		{
			var clientInfo = ClientInfos.FirstOrDefault(x => x.UID == uid);
			ClientInfos.Remove(clientInfo);
			Notifier.RemoveClient(uid);
		}

		public static ClientInfo GetClientInfo(Guid uid)
		{
			return ClientInfos.FirstOrDefault(x => x.UID == uid);
		}

		public static void StartRemoveInactiveClients(TimeSpan inactiveTime)
		{
			var thread = new Thread(() =>
				{
					while (true)
					{
						try
						{
							ClientInfos
								.Where(x => x.LastPollDateTime != default(DateTime) && DateTime.Now - x.LastPollDateTime > inactiveTime)
								.ToList()
								.ForEach(x => Remove(x.UID));
						}
						catch { }

						Thread.Sleep((int)inactiveTime.TotalMilliseconds / 10);
					}
				}) { Name = "RemoveInactiveClients", IsBackground = true };

			thread.Start();
		}
	}

	public class ClientInfo
	{
		public Guid UID { get; set; }
		public ClientCredentials ClientCredentials { get; set; }
		public int CallbackIndex { get; set; }
		public AutoResetEvent WaitEvent = new AutoResetEvent(false);
		public DateTime LastPollDateTime { get; set; }
		public Guid LayoutUID { get; set; }
	}
}