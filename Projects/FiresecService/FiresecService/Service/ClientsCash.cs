using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.ViewModels;
using System.Timers;

namespace FiresecService.Service
{
	public static class ClientsCash
	{
		public static List<FiresecService> FiresecServices { get; private set; }
		static FiresecManager MonitoringFiresecManager;
		static FiresecManager AdministratorFiresecManager;
		static System.Timers.Timer PingTimer;

		static ClientsCash()
		{
			FiresecServices = new List<FiresecService>();

			PingTimer = new System.Timers.Timer();
			PingTimer.Interval = 10000;
			PingTimer.Elapsed += new ElapsedEventHandler((source, e) => { PingClients(); });
			PingTimer.Start();
		}

		public static void InitializeComServers()
		{
			MonitoringFiresecManager = new FiresecManager(true);
			AdministratorFiresecManager = new FiresecManager(false);
		}

		public static bool Add(FiresecService firesecService)
		{
			switch (firesecService.ClientCredentials.ClientType)
			{
				case ClientType.Administrator:
					firesecService.FiresecManager = AdministratorFiresecManager;
					break;

				case ClientType.Monitor:
				case ClientType.Itv:
				case ClientType.Other:
					firesecService.FiresecManager = MonitoringFiresecManager;
					break;
			}

			if (!IsNew(firesecService))
				return false;

			var existingFiresecService = FiresecServices.FirstOrDefault(x => x.ClientIpAddress == firesecService.ClientIpAddress &&
				x.ClientCredentials.ClientType == firesecService.ClientCredentials.ClientType);
			if (existingFiresecService != null)
			{
				Remove(existingFiresecService);
				return false;
			}

			firesecService.FiresecManager.FiresecServices.Add(firesecService);
			FiresecServices.Add(firesecService);
			MainViewModel.Current.AddClient(firesecService);
			return true;
		}

		public static void Remove(FiresecService firesecService)
		{
			firesecService.FiresecManager.FiresecServices.Remove(firesecService);
			firesecService.FiresecManager = null;
			FiresecServices.Remove(firesecService);
			MainViewModel.Current.RemoveClient(firesecService.UID);
		}

		public static bool IsNew(FiresecService firesecService)
		{
			return !FiresecServices.Any(x => x.UID == firesecService.UID || x.ClientCredentials.ClientUID == firesecService.ClientCredentials.ClientUID);
		}

		public static void OnNewJournalRecord(JournalRecord journalRecord)
		{
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.OnNewJournalRecord(journalRecord);
			}
		}

		public static void OnConfigurationChanged()
		{
			MonitoringFiresecManager.ConvertStates();
			AdministratorFiresecManager.ConvertStates();
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.OnConfigurationChanged();
			}
		}

		static void PingClients()
		{
			PingTimer.Stop();
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.OnPing();
			}
			PingTimer.Start();
		}
	}
}