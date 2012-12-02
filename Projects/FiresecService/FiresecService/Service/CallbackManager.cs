using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		static List<CallbackResultItem> CallbackResultItems = new List<CallbackResultItem>();
		static int Index = 0;

		public static void Add(CallbackResult callbackResult)
		{
            lock (CallbackResultItems)
            {
                CallbackResultItems.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

                Index++;
                var callbackResultSaver = new CallbackResultItem()
                {
                    CallbackResult = callbackResult,
                    Index = Index,
                    DateTime = DateTime.Now
                };
                CallbackResultItems.Add(callbackResultSaver);

                ClientsManager.ClientInfos.ForEach(x=>x.WaitEvent.Set());
            }
		}

        public static List<CallbackResult> Get(ClientInfo clientInfo)
		{
            lock (CallbackResultItems)
            {
                var result = new List<CallbackResult>();
                if (clientInfo.IsDisconnecting)
                {
                    var callbackResult = new CallbackResult()
                    {
                        CallbackResultType = CallbackResultType.Disconnecting
                    };
                    result.Add(callbackResult);
                    return result;
                }
                foreach (var callbackResultSaver in CallbackResultItems)
                {
                    if (callbackResultSaver.Index > clientInfo.CallbackIndex)
                    {
                        result.Add(callbackResultSaver.CallbackResult);
                    }
                }
                clientInfo.CallbackIndex = Index;
                return result;
            }
		}
	}

	public class CallbackResultItem
	{
		public CallbackResult CallbackResult { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}