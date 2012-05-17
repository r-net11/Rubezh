using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.ViewModels;
using System.Runtime.Serialization;
using System.IO;

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
		static object locker = new object();

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

		public static void CopyConfigurationForAllClients(FiresecService firesecService)
		{
			foreach (var serviceInstance in _serviceInstances)
			{
				if (serviceInstance.UID != firesecService.UID)
				{
					DeviceConfiguration clonedDeviceConfiguration = null;

					var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
					using (var memoryStream = new MemoryStream())
					{
						dataContractSerializer.WriteObject(memoryStream, firesecService.FiresecManager.ConfigurationManager.DeviceConfiguration);
						memoryStream.Position = 0;
						clonedDeviceConfiguration = (DeviceConfiguration)dataContractSerializer.ReadObject(memoryStream);
					}

					clonedDeviceConfiguration.Update();
					serviceInstance.FiresecManager.ConfigurationManager.DeviceConfiguration = clonedDeviceConfiguration;
				}
			}
		}
	}
}