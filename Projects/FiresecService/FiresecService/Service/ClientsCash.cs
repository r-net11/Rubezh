using System.Collections.Generic;
using System.Linq;
using System.Timers;
using FiresecAPI.Models;
using FiresecService.Processor;
using FiresecService.ViewModels;
using Common;
using System;

namespace FiresecService.Service
{
	public static class ClientsCash
	{
		public static List<FiresecService> FiresecServices { get; private set; }
		public static FiresecManager MonitoringFiresecManager { get; private set; }
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

		public static bool InitializeComServers()
		{
			try
			{
				MainViewModel.Current.UpdateCurrentStatus("Соединение с ядром для мониторинга");
				MonitoringFiresecManager = new FiresecManager(true);
				MainViewModel.Current.UpdateCurrentStatus("Соединение с ядром для администрирования");
				AdministratorFiresecManager = new FiresecManager(false);
				return (MonitoringFiresecManager.IsConnectedToComServer && AdministratorFiresecManager.IsConnectedToComServer);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientsCash.InitializeComServers");
				return false;
			}
		}

		public static void Add(FiresecService firesecService)
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
				return;

			var existingFiresecService = FiresecServices.FirstOrDefault(x => x.ClientIpAddress == firesecService.ClientIpAddress &&
				x.ClientCredentials.ClientType == firesecService.ClientCredentials.ClientType);
			if (existingFiresecService != null)
			{
				Remove(existingFiresecService);
			}

			firesecService.FiresecManager.FiresecServices.Add(firesecService);
			FiresecServices.Add(firesecService);
			MainViewModel.Current.AddClient(firesecService);
		}

		public static void Remove(FiresecService firesecService)
		{
			if (firesecService.FiresecManager != null)
			{
				firesecService.FiresecManager.FiresecServices.Remove(firesecService);
				firesecService.FiresecManager = null;
			}
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
			MonitoringFiresecManager.ConvertStates();
			AdministratorFiresecManager.ConvertStates();
			foreach (var firesecServices in FiresecServices)
			{
				firesecServices.CallbackWrapper.ConfigurationChanged();
			}
		}

		static void PingClients()
		{
			PingTimer.Stop();
			foreach (var firesecService in FiresecServices)
			{
				var clientUID = firesecService.CallbackWrapper.Ping();
				if (clientUID != firesecService.ClientCredentials.ClientUID)
				{
					Logger.Info("ClientsCash.PingClients clientUID != firesecService.ClientCredentials.ClientUID");
				}
			}
			PingTimer.Start();
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