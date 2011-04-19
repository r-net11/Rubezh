using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public class FiresecEventAggregator
    {
        public static event Action<int, string> NewEventAvaliable;
        public static void OnNewEventAvaliable(int eventMask, string obj)
        {
            if (NewEventAvaliable != null)
                NewEventAvaliable(eventMask, obj);
        }
    }
}
