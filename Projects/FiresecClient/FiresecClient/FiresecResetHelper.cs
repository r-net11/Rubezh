using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient.Models;

namespace FiresecClient
{
    public static class FiresecResetHelper
    {
        public static void ResetOne(string deviceId, string stateName)
        {
            var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
            var innerState = deviceState.InnerStates.First(x => x.Name == stateName);

            Firesec.CoreState.config coreState = new Firesec.CoreState.config();
            coreState.dev = new Firesec.CoreState.devType[1];
            coreState.dev[0] = new Firesec.CoreState.devType();
            coreState.dev[0].name = deviceState.PlaceInTree;
            coreState.dev[0].state = new Firesec.CoreState.stateType[1];
            coreState.dev[0].state[0] = new Firesec.CoreState.stateType();
            coreState.dev[0].state[0].id = innerState.Id;

            FiresecInternalClient.ResetStates(coreState);
        }

        public static void ResetMany(List<ResetItem> resetItems)
        {
            List<Firesec.CoreState.devType> innerDevices = new List<Firesec.CoreState.devType>();

            foreach (var resetItem in resetItems)
            {
                if (resetItem == null)
                    continue;

                var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == resetItem.DeviceId);

                Firesec.CoreState.devType innerDevice = new Firesec.CoreState.devType();
                innerDevice.name = deviceState.PlaceInTree;

                List<Firesec.CoreState.stateType> innerStates = new List<Firesec.CoreState.stateType>();

                foreach (var state in resetItem.States)
                {
                    var innerState = deviceState.InnerStates.First(x => x.Name == state);
                    innerStates.Add(new Firesec.CoreState.stateType() { id = innerState.Id });
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
