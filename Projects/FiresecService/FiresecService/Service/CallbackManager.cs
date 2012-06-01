using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.ViewModels;
using System.Runtime.Serialization;
using System.IO;
using Common;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		static CallbackManager()
		{
			FiresecServices = new List<FiresecService>();
		}

		static List<FiresecService> FiresecServices;
		static List<FiresecService> FailedFiresecServices;

		public static void Add(FiresecService firesecService)
		{
			FiresecServices.Add(firesecService);
		}

		public static void Remove(FiresecService firesecService)
		{
			FiresecServices.Remove(firesecService);
		}

		static void Clean()
		{
			try
			{
				foreach (var firesecServices in FailedFiresecServices)
				{
					MainViewModel.Current.RemoveConnection(firesecServices.UID);
					FiresecServices.Remove(firesecServices);
					ServiceCash.Free(firesecServices.FiresecManager);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове CallbackManager.Clean");
			}
		}

		static void SafeCall(Action<FiresecService> action, bool subscribeRequired = true)
		{
			FailedFiresecServices = new List<FiresecService>();
			foreach (var firesecServices in FiresecServices)
			{
				if (!subscribeRequired || firesecServices.IsSubscribed)
					try
					{
						action(firesecServices);
					}
					catch (Exception e)
					{
						Logger.Error(e, "Исключение при вызове CallbackManager.SafeCall");
						FailedFiresecServices.Add(firesecServices);
					}
			}

			Clean();
		}

		public static void OnNewJournalRecord(JournalRecord journalRecord)
		{
			SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); });
		}

		public static void OnConfigurationChanged()
		{
			SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); });
		}

		public static void Ping()
		{
			FailedFiresecServices = new List<FiresecService>();
			foreach (var firesecServices in FiresecServices)
			{
				try
				{
					var clientUID = firesecServices.FiresecCallbackService.Ping();
					if (firesecServices.ClientUID != clientUID)
					{
						FailedFiresecServices.Add(firesecServices);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове CallbackManager.Ping");
					FailedFiresecServices.Add(firesecServices);
				}
			}

			Clean();
		}
	}
}