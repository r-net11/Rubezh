using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public class FiresecEventAggregator
    {
        public static event Action<int> NewEventAvaliable;
        public static void OnNewEventAvaliable(int eventMask)
        {
            if (NewEventAvaliable != null)
                NewEventAvaliable(eventMask);
        }
    }
}
