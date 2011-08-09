using System;

namespace Firesec
{
    public static class FiresecEventAggregator
    {
        public static event Action<int> NewEventAvaliable;
        public static void OnNewEventAvaliable(int eventMask)
        {
            if (NewEventAvaliable != null)
                NewEventAvaliable(eventMask);
        }
    }
}