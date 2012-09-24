using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	public static class ClientsCash
	{
		public static List<FiresecService> FiresecServices { get; private set; }

		static ClientsCash()
		{
			FiresecServices = new List<FiresecService>();
		}

		public static void Add(FiresecService firesecService)
		{
			if (!IsNew(firesecService))
				return;

			var existingFiresecService = FiresecServices.FirstOrDefault(x => x.ClientIpAddress == firesecService.ClientIpAddress &&
				x.ClientCredentials.ClientType == firesecService.ClientCredentials.ClientType);
			if (existingFiresecService != null)
			{
				Remove(existingFiresecService);
			}
			FiresecServices.Add(firesecService);
			MainViewModel.Current.AddClient(firesecService);
		}

		public static void Remove(FiresecService firesecService)
		{
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
                firesecServices.CallbackNewJournal(new List<JournalRecord>() { journalRecord });
            }
        }

		public static void OnConfigurationChanged()
		{
			foreach (var firesecServices in FiresecServices)
			{
                firesecServices.CallbackConfigurationChanged();
			}
		}
	}
}