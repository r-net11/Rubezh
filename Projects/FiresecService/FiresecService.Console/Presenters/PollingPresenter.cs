using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService
{
	static class PollingPresenter
	{
		public static List<PollingItem> PollingItems { get; private set; }
		static PollingPresenter()
		{
			PollingItems = new List<PollingItem>();
		}
		public static void AddOrUpdate(Guid clientUID)
		{
			var clientInfo = FiresecService.Service.ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
			var pollingItem = PollingItems.FirstOrDefault(x => x.ClientUID == clientUID);
			var connection = ConnectionsPresenter.Connections.FirstOrDefault(x => x.UID == clientUID);

			string clientType = "", ipAddress = "", userName = "";
			if (connection != null)
			{
				clientType = connection.ClientType;
				ipAddress = connection.IpAddress;
				userName = connection.FriendlyUserName;
			}

			var callbackIndex = clientInfo == null ?
				-1 :
				clientInfo.CallbackIndex;

			if (pollingItem == null)
			{
				PollingItems.Add(new PollingItem(clientUID, clientType, ipAddress, userName, callbackIndex));
			}
			else
			{
				pollingItem.CallbackIndex = callbackIndex;
				pollingItem.LastPollTime = DateTime.Now;
			}

			PageController.OnPageChanged(Page.Polling);
		}
	}

	class PollingItem
	{
		public Guid UID { get; private set; }
		public string ClientType { get; private set; }
		public string IpAddress { get; private set; }
		public string UserName { get; private set; }
		public Guid ClientUID { get; private set; }
		public DateTime FirstPollTime { get; private set; }
		public DateTime LastPollTime { get; set; }
		public int CallbackIndex { get; set; }


		public PollingItem(Guid clientUID, string clientType, string ipAddress, string userName, int callbackIndex)
		{
			UID = Guid.NewGuid();
			ClientUID = clientUID;
			ClientType = clientType;
			IpAddress = ipAddress;
			UserName = userName;
			FirstPollTime = LastPollTime = DateTime.Now;
			CallbackIndex = callbackIndex;
		}
	}
}
