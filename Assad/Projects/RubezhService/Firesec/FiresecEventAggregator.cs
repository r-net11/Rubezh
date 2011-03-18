using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public class FiresecEventAggregator
    {
        public static event Action<string, CoreState.config> StateChanged;
        public static void OnStateChanged(string coreStateString, CoreState.config coreState)
        {
            if (StateChanged != null)
                StateChanged(coreStateString, coreState);
        }

        public static event Action<string, DeviceParams.config> ParametersChanged;
        public static void OnParametersChanged(string coreParametersString, DeviceParams.config coreParameters)
        {
            if (ParametersChanged != null)
                ParametersChanged(coreParametersString, coreParameters);
        }
    }
}
