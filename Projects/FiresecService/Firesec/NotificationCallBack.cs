
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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