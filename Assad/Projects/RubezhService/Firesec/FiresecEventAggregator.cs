using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public class FiresecEventAggregator
    {
        public static event Action<string, CoreState.config> NewEvent;
        public static void OnNewEvent(string coreStateString, CoreState.config coreState)
        {
            if (NewEvent != null)
                NewEvent(coreStateString, coreState);
        }
    }
}
