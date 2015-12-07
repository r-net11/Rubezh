﻿using FiresecService.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FiresecService.Service
{
	public static class ClientsManager
	{
		public static List<ClientInfo> ClientInfos = new List<ClientInfo>();

		public static bool Add(Guid uid, ClientCredentials clientCredentials)
		{
			if (ClientInfos.Any(x => x.UID == uid))
				return false;

			var result = true;
			var existingClientInfo = ClientInfos.FirstOrDefault(x => x.ClientCredentials.UniqueId == clientCredentials.UniqueId);
			if (existingClientInfo != null)
			{
				Remove(existingClientInfo.UID);
				result = false;
			}

			var clientInfo = new ClientInfo();
			clientInfo.UID = uid;
			clientInfo.ClientCredentials = clientCredentials;
			//clientInfo.CallbackIndex = CallbackManager.Index;
			ClientInfos.Add(clientInfo);

			MainViewModel.Current.AddClient(clientCredentials);
			return result;
		}

		public static void Remove(Guid uid)
		{
			var clientInfo = ClientInfos.FirstOrDefault(x => x.UID == uid);
			ClientInfos.Remove(clientInfo);
			MainViewModel.Current.RemoveClient(uid);
		}

		public static ClientInfo GetClientInfo(Guid uid)
		{
			return ClientInfos.FirstOrDefault(x => x.UID == uid);
		}

		public static ClientCredentials GetClientCredentials(Guid uid)
		{
			var clientInfo = ClientInfos.FirstOrDefault(x => x.UID == uid);
			if (clientInfo != null)
			{
				return clientInfo.ClientCredentials;
			}
			return null;
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
		public bool IsDisconnecting { get; set; }
		public DateTime LastPollDateTime { get; set; }
	}
}