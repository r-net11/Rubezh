﻿namespace Firesec
{
    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            FiresecEventAggregator.OnNewEventAvaliable(EventMask);
        }

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            return true;
        }
    }
}