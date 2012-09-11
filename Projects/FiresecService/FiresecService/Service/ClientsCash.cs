using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
//using FiresecService.Processor;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	public static class ClientsCash
	{
		public static List<FiresecService> FiresecServices { get; private set; }
        //public static FiresecManager MonitoringFiresecManager { get; private set; }
        //static FiresecManager AdministratorFiresecManager;

		static ClientsCash()
		{
			FiresecServices = new List<FiresecService>();
		}

        //public static void InitializeComServers()
        //{
        //    try
        //    {
        //        UILogger.Log("Перезапуск Socket Server");
        //        SocketServerHelper.Stop();
        //        UILogger.Log("Загрузка драйвера для мониторинга");
        //        MonitoringFiresecManager = new FiresecManager(true);
        //        UILogger.Log("Загрузка драйвера для администрирования");
        //        AdministratorFiresecManager = new FiresecManager(false);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e, "ClientsCash.InitializeComServers");
        //    }
        //}

		public static void Add(FiresecService firesecService)
		{
            //switch (firesecService.ClientCredentials.ClientType)
            //{
            //    case ClientType.Administrator:
            //        firesecService.FiresecManager = AdministratorFiresecManager;
            //        break;

            //    case ClientType.Monitor:
            //    case ClientType.Itv:
            //    case ClientType.Other:
            //        firesecService.FiresecManager = MonitoringFiresecManager;
            //        break;
            //}

			if (!IsNew(firesecService))
				return;

			var existingFiresecService = FiresecServices.FirstOrDefault(x => x.ClientIpAddress == firesecService.ClientIpAddress &&
				x.ClientCredentials.ClientType == firesecService.ClientCredentials.ClientType);
			if (existingFiresecService != null)
			{
				Remove(existingFiresecService);
			}
            //if (firesecService.FiresecManager != null)
            //{
            //    firesecService.FiresecManager.FiresecServices.Add(firesecService);
            //}
			FiresecServices.Add(firesecService);
			MainViewModel.Current.AddClient(firesecService);
		}

		public static void Remove(FiresecService firesecService)
		{
            //if (firesecService.FiresecManager != null)
            //{
            //    firesecService.FiresecManager.FiresecServices.Remove(firesecService);
            //    firesecService.FiresecManager = null;
            //}
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
                firesecServices.CallbackWrapper.NewJournalRecords(new List<JournalRecord>() { journalRecord });
            }
        }

		public static void OnConfigurationChanged()
		{
			//MonitoringFiresecManager.ConvertStates();
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.ConfigurationChanged();
			}
		}

		static void PingClients()
		{
			foreach (var firesecService in FiresecServices)
			{
				var clientUID = firesecService.CallbackWrapper.Ping();
				if (clientUID != firesecService.ClientCredentials.ClientUID)
				{
					Logger.Info("ClientsCash.PingClients clientUID != firesecService.ClientCredentials.ClientUID");
				}
			}
		}

		public static void NotifyClients(string message)
		{
			foreach (var firesecService in FiresecServices)
			{
				firesecService.CallbackWrapper.Notify(message);
			}
		}
	}
}