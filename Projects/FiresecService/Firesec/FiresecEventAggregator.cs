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

        public static event Action<int, string, int, int> Progress;
        public static void OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            if (Progress != null)
                Progress(Stage, Comment, PercentComplete, BytesRW);
        }
    }
}