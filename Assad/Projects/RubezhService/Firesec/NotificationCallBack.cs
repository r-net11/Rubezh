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
            string coreStateString = NativeComServer.GetCoreState();
            CoreState.config coreState = ComServer.GetCoreState();
            FiresecEventAggregator.OnNewEvent(coreStateString, coreState);
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