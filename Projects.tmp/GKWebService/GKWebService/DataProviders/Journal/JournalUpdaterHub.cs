using GKWebService.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.DataProviders
{
	[HubName("journalUpdaterHub")]
	public class JournalUpdaterHub : Hub
	{
		public static JournalUpdaterHub Instance { get; private set; }

		public JournalUpdaterHub()
		{
			Instance = this;
		}

		public void BroadcastNewJournalItems(List<JournalItem> apiItems)
		{
			var journalItems = apiItems.Select(x => new JournalModel(x)).ToList();
			Clients.All.updateJournalItems(journalItems);
		}
	}
}