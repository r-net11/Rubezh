using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComServer
{
    public class ComEventAggregator
    {
        public static event Action<string, CoreState.config> ComEvent;
        public static void OnComEvent(string coreStateString, CoreState.config coreState)
        {
            if (ComEvent != null)
                ComEvent(coreStateString, coreState);
        }
    }
}
