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

        public static int IntContinueProgress = 1;

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            try
            {
                var continueProgress = IntContinueProgress == 1;
                IntContinueProgress = 1;
                ProgressHelper.ProcessProgress(Stage, Comment, PercentComplete, BytesRW);
                return continueProgress;
            }
            catch (Exception e)
            {
                Trace.WriteLine("OnProgress: Exception");
                return false;
            }
        }
    }
}