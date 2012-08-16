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
			var innerDevices = new List<Firesec.CoreState.devType>();

			if (resetItems == null)
			{
				Logger.Error("AlarmWatcher.UpdateValveTimer: resetItems = null");
				return;
			}
			foreach (var resetItem in resetItems)
			{
				if (resetItem == null)
					continue;

				var deviceState = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == resetItem.DeviceUID);
				if (deviceState == null)
				{
					Logger.Error("AlarmWatcher.UpdateValveTimer: deviceState = null");
					return;
				}

				var innerStates = new List<Firesec.CoreState.stateType>();

				if (resetItem.StateNames == null)
				{
					Logger.Error("AlarmWatcher.UpdateValveTimer: resetItem.StateNames = null");
					return;
				}
				foreach (var stateName in resetItem.StateNames)
				{
					var deviceDriverState = deviceState.States.FirstOrDefault(x => x.DriverState.Name == stateName).DriverState;
					if (deviceDriverState == null)
					{
						Logger.Error("AlarmWatcher.UpdateValveTimer: deviceDriverState = null");
						continue;
					}
					innerStates.Add(new Firesec.CoreState.stateType() { id = deviceDriverState.Id });
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