
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	public static class ClientsCash
	{
		public static List<FiresecService> FiresecServices { get; private set; }
		static FiresecManager MonitoringFiresecManager;
		static FiresecManager AdministratorFiresecManager;

		static ClientsCash()
		{
			MonitoringFiresecManager = new FiresecManager();
			AdministratorFiresecManager = new FiresecManager();
			FiresecServices = new List<FiresecService>();
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

			var existingFiresecService = FiresecServices.FirstOrDefault(x => x.ClientAddress == firesecService.ClientAddress);
			if (existingFiresecService != null)
			{
				Remove(firesecService);
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
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.OnConfigurationChanged();
			}
		}
	}
}