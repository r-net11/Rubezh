using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		static CallbackManager()
		{
			_serviceInstances = new List<FiresecService>();
		}

		static List<FiresecService> _serviceInstances;
		static List<FiresecService> _failedServiceInstances;

		public static void Add(FiresecService firesecService)
		{
			_serviceInstances.Add(firesecService);
		}

		public static void Remove(FiresecService firesecService)
		{
			_serviceInstances.Remove(firesecService);
		}

		static void Clean()
		{
			try
			{
				foreach (var failedServiceInstance in _failedServiceInstances)
				{
					MainViewModel.Current.RemoveConnection(failedServiceInstance.UID);
					_serviceInstances.Remove(failedServiceInstance);
				}
			}
			catch { ;}
		}

		static void SafeCall(Action<FiresecService> action)
		{
			lock (FiresecService.Locker)
			{
				_failedServiceInstances = new List<FiresecService>();
				foreach (var serviceInstance in _serviceInstances)
				{
					if (serviceInstance.IsSubscribed)
						try
						{
							action(serviceInstance);
						}
						catch
						{
							_failedServiceInstances.Add(serviceInstance);
						}
				}

				Clean();
			}
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
			SafeCall((x) => { x.FiresecCallbackService.Ping(); });
		}
	}
}