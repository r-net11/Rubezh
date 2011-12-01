using System;

namespace Firesec2
{
    public static class FiresecEventAggregator
    {
        public static event Action<int> NewEventAvaliable;
        public static void OnNewEventAvaliable(int eventMask)
        {
            if (NewEventAvaliable != null)
                NewEventAvaliable(eventMask);
        }

        //public static event FiresecEventAggregator.ProgressDelegate Progress;
        public static bool OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            return DispatcherFiresecClient.ProcessProgress(Stage, Comment, PercentComplete, BytesRW);
            //if (Progress != null)
            //    return Progress(Stage, Comment, PercentComplete, BytesRW);
            //return false;
        }

        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);
    }
}