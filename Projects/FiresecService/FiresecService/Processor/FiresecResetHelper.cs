using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecService.Service;

namespace FiresecService.Processor
{
	public class FiresecResetHelper
	{
		FiresecManager FiresecManager;

		public FiresecResetHelper(FiresecManager firesecManager)
		{
			FiresecManager = firesecManager;
		}

		public void ResetStates(List<ResetItem> resetItems)
		{
			if (FiresecManager == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: FiresecManager = null");
				return;
			}
			if (FiresecManager.FiresecSerializedClient == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: FiresecManager.FiresecSerializedClient = null");
				return;
			}
			if (resetItems == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: resetItems = null");
				return;
			}
			if (ConfigurationCash.DeviceConfigurationStates == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: ConfigurationCash.DeviceConfigurationStates = null");
				return;
			}
			if (ConfigurationCash.DeviceConfigurationStates.DeviceStates == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: ConfigurationCash.DeviceConfigurationStates.DeviceStates = null");
				return;
			}

			var innerDevices = new List<Firesec.CoreState.devType>();

			foreach (var resetItem in resetItems)
			{
				if (resetItem == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: resetItem = null");
					continue;
				}

				var deviceState = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == resetItem.DeviceUID);
				if (deviceState == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: deviceState = null");
					continue;
				}

				var innerStates = new List<Firesec.CoreState.stateType>();

				if (resetItem.StateNames == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: resetItem.StateNames = null");
					continue;
				}
				if (deviceState.States == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: deviceState.States = null");
					continue;
				}
				foreach (var stateName in resetItem.StateNames)
				{
					if (stateName == null)
					{
						Logger.Error("FiresecResetHelper.ResetStates: stateName = null");
						continue;
					}
					var deviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.Name == stateName);
					if (deviceDriverState == null)
					{
						Logger.Error("FiresecResetHelper.ResetStates: deviceDriverState = null");
						continue;
					}
					var driverState = deviceDriverState.DriverState;
					if (driverState == null)
					{
						Logger.Error("FiresecResetHelper.ResetStates: deviceDriverState.DriverState = null");
						continue;
					}
					innerStates.Add(new Firesec.CoreState.stateType() { id = driverState.Id });
				}
				var innerDevice = new Firesec.CoreState.devType()
				{
					name = deviceState.PlaceInTree,
					state = innerStates.ToArray()
				};
				innerDevices.Add(innerDevice);
			}

			var coreState = new Firesec.CoreState.config()
			{
				dev = innerDevices.ToArray()
			};
			FiresecManager.FiresecSerializedClient.ResetStates(coreState);
		}
	}
}