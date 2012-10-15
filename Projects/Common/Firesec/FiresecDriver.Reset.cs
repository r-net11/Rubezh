using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using System.Threading;

namespace Firesec
{
	public partial class FiresecDriver
	{
		public void ResetStates(List<ResetItem> resetItems)
		{
			if (FiresecSerializedClient == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: FiresecManager.FiresecSerializedClient = null");
				return;
			}
			if (resetItems == null)
			{
				Logger.Error("FiresecResetHelper.ResetStates: resetItems = null");
				return;
			}

			var innerDevices = new List<Firesec.Models.CoreState.devType>();

			foreach (var resetItem in resetItems)
			{
				if (resetItem == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: resetItem = null");
					continue;
				}
				if (resetItem.DeviceState == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: resetItem.DeviceState = null");
					continue;
				}

				var innerStates = new List<Firesec.Models.CoreState.stateType>();

				if (resetItem.States == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: resetItem.States = null");
					continue;
				}
				if (resetItem.DeviceState.States == null)
				{
					Logger.Error("FiresecResetHelper.ResetStates: deviceState.States = null");
					continue;
				}
				foreach (var deviceDriverState in resetItem.States)
				{
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
					innerStates.Add(new Firesec.Models.CoreState.stateType() { id = driverState.Id });
				}
				var innerDevice = new Firesec.Models.CoreState.devType()
				{
					name = resetItem.DeviceState.Device.PlaceInTree,
					state = innerStates.ToArray()
				};
				innerDevices.Add(innerDevice);
			}

			var coreState = new Firesec.Models.CoreState.config()
			{
				dev = innerDevices.ToArray()
			};

			var thread = new Thread(new ThreadStart(() =>
				{
					FiresecSerializedClient.ResetStates(coreState);
				}));
			thread.Start();
		}
	}
}