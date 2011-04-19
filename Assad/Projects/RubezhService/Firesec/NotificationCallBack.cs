
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
            bool evmNewEvents = ((EventMask & 1) == 1);
            bool evmStateChanged = ((EventMask & 2) == 2);
            bool evmConfigChanged = ((EventMask & 4) == 4);
            bool evmDeviceParamsUpdated = ((EventMask & 8) == 8);
            bool evmPong = ((EventMask & 16) == 16);
            bool evmDatabaseChanged = ((EventMask & 32) == 32);
            bool evmReportsChanged = ((EventMask & 64) == 64);
            bool evmSoundsChanged = ((EventMask & 128) == 128);
            bool evmLibraryChanged = ((EventMask & 256) == 256);
            bool evmPing = ((EventMask & 512) == 512);
            bool evmIgnoreListChanged = ((EventMask & 1024) == 1024);
            bool evmEventViewChanged = ((EventMask & 2048) == 2048);

            string obj;

            if (evmStateChanged)
            {
                obj = NativeFiresecClient.GetCoreState();
                FiresecEventAggregator.OnNewEventAvaliable(2, obj);
            }
            if (evmDeviceParamsUpdated)
            {
                obj = NativeFiresecClient.GetCoreDeviceParams();
                FiresecEventAggregator.OnNewEventAvaliable(8, obj);
            }
            if (evmNewEvents)
            {
                obj = NativeFiresecClient.ReadEvents(0, 100);
                FiresecEventAggregator.OnNewEventAvaliable(1, obj);
            }
        }
    }
}