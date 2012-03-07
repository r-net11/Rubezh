using System.Diagnostics;
using System;
namespace Firesec
{
    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            FiresecEventAggregator.OnNewEventAvaliable(EventMask);
        }

        public static bool ContinueProgress = true;

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            try
            {
                FiresecEventAggregator.OnProgress(Stage, Comment, PercentComplete, BytesRW);
                var result = ContinueProgress;
                ContinueProgress = true;
                Trace.WriteLine("OnProgress: " + result.ToString());
                return result;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }

    //public static class ProgressState
    //{
    //    public static bool ContinueProgress;
    //}
}