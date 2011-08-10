using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService
{
    public static class FiresecResetHelper
    {
        public static void ResetMany(List<ResetItem> resetItems)
        {
            List<Firesec.CoreState.devType> innerDevices = new List<Firesec.CoreState.devType>();

            foreach (var resetItem in resetItems)
            {
                if (resetItem == null)
                    continue;

                var deviceState = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == resetItem.DeviceId);

                Firesec.CoreState.devType innerDevice = new Firesec.CoreState.devType();
                innerDevice.name = deviceState.PlaceInTree;

                List<Firesec.CoreState.stateType> innerStates = new List<Firesec.CoreState.stateType>();

                foreach (var stateName in resetItem.StateNames)
                {
                    var deviceDriverState = deviceState.States.First(x => x.DriverState.Name == stateName).DriverState;
                    innerStates.Add(new Firesec.CoreState.stateType() { id = deviceDriverState.Id });
                }
                innerDevice.state = innerStates.ToArray();
                innerDevices.Add(innerDevice);
            }

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = innerDevices.ToArray();
            FiresecInternalClient.ResetStates(coreState);
        }
    }
}
