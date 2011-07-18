

namespace Firesec
{
    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            FiresecEventAggregator.OnNewEventAvaliable(EventMask);
        }
    }
}