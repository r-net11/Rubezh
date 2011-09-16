using System.Diagnostics;
namespace Firesec
{
    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            FiresecEventAggregator.OnNewEventAvaliable(EventMask);
        }

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            FiresecEventAggregator.OnProgress(Stage, Comment, PercentComplete, BytesRW);

            Trace.WriteLine("Progress: Stage=" + Stage.ToString() + " Comment:" + Comment +
                " PercentComplete:" + PercentComplete.ToString() + " BytesRW:" + BytesRW.ToString());
            return true;
        }
    }
}