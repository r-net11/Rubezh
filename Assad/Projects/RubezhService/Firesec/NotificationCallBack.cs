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
            Trace.WriteLine("NewEventsAvailable");

            bool evmNewEvents = ((EventMask & 1) == 1);
            bool evmStateChanged = ((EventMask & 2) == 1);
            bool evmConfigChanged = ((EventMask & 4) == 1);
            bool evmDeviceParamsUpdated = ((EventMask & 8) == 1);
            bool evmPong = ((EventMask & 10) == 1);
            bool evmDatabaseChanged = ((EventMask & 20) == 1);
            bool evmReportsChanged = ((EventMask & 40) == 1);
            bool evmSoundsChanged = ((EventMask & 80) == 1);
            bool evmLibraryChanged = ((EventMask & 100) == 1);
            bool evmPing = ((EventMask & 200) == 1);
            bool evmIgnoreListChanged = ((EventMask & 400) == 1);
            bool evmEventViewChanged = ((EventMask & 800) == 1);

            if (evmStateChanged)
            {
                CoreState.config coreState = ComServer.GetCoreState();
                FiresecEventAggregator.OnNewEvent(ComServer.CoreStateString, coreState);
            }
        }
    }
}

  //evmNewEvents           = $0001;
  //evmStateChanged        = $0002;
  //evmConfigChanged       = $0004;
  //evmDeviceParamsUpdated = $0008;
  //evmPong                = $0010;
  //evmDatabaseChanged     = $0020;
  //evmReportsChanged      = $0040;
  //evmSoundsChanged       = $0080;
  //evmLibraryChanged      = $0100;
  //evmPing                = $0200;
  //evmIgnoreListChanged   = $0400;
  //evmEventViewChanged    = $0800;