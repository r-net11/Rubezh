using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Firesec;
using FiresecAPI;

namespace FiresecService
{
    public class FiresecResetHelper
    {
		FiresecManager FiresecManager;

		public FiresecResetHelper(FiresecManager firesecManager)
		{
			FiresecManager = firesecManager;
		}

        public FiresecOperationResult<bool> ResetStates(List<ResetItem> resetItems)
        {
            var innerDevices = new List<Firesec.CoreState.devType>();

            foreach (var resetItem in resetItems)
            {
                if (resetItem == null)
                    continue;

                var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == resetItem.DeviceUID);

                var innerStates = new List<Firesec.CoreState.stateType>();

                foreach (var stateName in resetItem.StateNames)
                {
                    var deviceDriverState = deviceState.States.First(x => x.DriverState.Name == stateName).DriverState;
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
			return FiresecManager.FiresecSerializedClient.ResetStates(coreState);
        }
    }
}