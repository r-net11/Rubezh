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

        public static void OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            DispatcherFiresecClient.ProcessProgress(Stage, Comment, PercentComplete, BytesRW);
        }

        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);
    }
}