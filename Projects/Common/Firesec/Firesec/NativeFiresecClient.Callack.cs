using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;
using FiresecAPI.Models;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        public void NewEventsAvailable(int eventMask)
        {
			needToRead = true;

            bool evmNewEvents = ((eventMask & 1) == 1);
            bool evmStateChanged = ((eventMask & 2) == 2);
            bool evmConfigChanged = ((eventMask & 4) == 4);
            bool evmDeviceParamsUpdated = ((eventMask & 8) == 8);
            bool evmPong = ((eventMask & 16) == 16);
            bool evmDatabaseChanged = ((eventMask & 32) == 32);
            bool evmReportsChanged = ((eventMask & 64) == 64);
            bool evmSoundsChanged = ((eventMask & 128) == 128);
            bool evmLibraryChanged = ((eventMask & 256) == 256);
            bool evmPing = ((eventMask & 512) == 512);
            bool evmIgnoreListChanged = ((eventMask & 1024) == 1024);
            bool evmEventViewChanged = ((eventMask & 2048) == 2048);

			if (evmStateChanged)
			{
				needToReadStates = true;
			}
			if (needToReadParameters)
			{
				needToReadParameters = true;
			}
			if (evmNewEvents)
			{
				needToReadJournal = true;
			}

			CheckForRead();
			return;

            SuspendOperationQueueEvent = new AutoResetEvent(false);
            try
            {
                if (evmStateChanged)
                {
					var result = SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
                    if (result != null && result.Result != null)
                    {
                        var coreState = ConvertResultData<Firesec.Models.CoreState.config>(result);
                        if (coreState.Result != null)
                        {
                            if (StateChanged != null)
                                StateChanged(coreState.Result);
                        }
                    }
                }

                if (evmDeviceParamsUpdated)
                {
					var result = SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
                    if (result != null && result.Result != null)
                    {
                        var coreParameters = ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
                        if (coreParameters.Result != null)
                        {
                            if (ParametersChanged != null)
                                ParametersChanged(coreParameters.Result);
                        }
                    }
                }

                if (evmNewEvents)
                {
                    ;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "NativeFiresecClient.NewEventsAvailable");
            }
            finally
            {
                SuspendOperationQueueEvent.Set();
                SuspendOperationQueueEvent = null;
            }

            if (NewEventAvaliable != null)
                NewEventAvaliable(eventMask);
        }

        OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
        {
            var resultData = new OperationResult<T>();
            resultData.HasError = result.HasError;
            resultData.Error = result.Error;
            if (result.HasError == false)
                resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
            return resultData;
        }

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
			CheckForRead();
			//return true;

            try
            {
                if (ProgressEvent != null)
                {
                    return ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
                return false;
            }
        }

		public event Action<List<JournalRecord>> NewJournalRecord;
        public event Action<Firesec.Models.CoreState.config> StateChanged;
        public event Action<Firesec.Models.DeviceParameters.config> ParametersChanged;
        public event Action<int> NewEventAvaliable;
        public event Func<int, string, int, int, bool> ProgressEvent;
    }
}