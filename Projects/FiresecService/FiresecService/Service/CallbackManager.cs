using StrazhAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		private static List<CallbackResultItem> CallbackResultItems = new List<CallbackResultItem>();
		private static int Index = 0;

		public static void Add(CallbackResult callbackResult, Guid? clientUID = null)
		{
			lock (CallbackResultItems)
			{
				CallbackResultItems.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

				Index++;
				var newCallbackResultItem = new CallbackResultItem()
				{
					CallbackResult = callbackResult,
					Index = Index,
					DateTime = DateTime.Now,
					ClientUID = clientUID,
				};
				CallbackResultItems.Add(newCallbackResultItem);

				if (callbackResult.CallbackResultType == CallbackResultType.SKDProgress)
				{
					var callbackResultItem = CallbackResultItems.FirstOrDefault(x => x.CallbackResult.SKDProgressCallback != null && x.CallbackResult.SKDProgressCallback.UID == callbackResult.SKDProgressCallback.UID);
					if (callbackResultItem != null)
					{
						if (callbackResult.SKDProgressCallback.LastActiveDateTime > callbackResultItem.CallbackResult.SKDProgressCallback.LastActiveDateTime)
						{
							CallbackResultItems.Remove(callbackResultItem);
						}
					}
				}

				ClientsManager.ClientInfos.ForEach(x => x.WaitEvent.Set());
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
				foreach (var callbackResultItem in CallbackResultItems)
				{
					if (callbackResultItem.Index > clientInfo.CallbackIndex)
					{
						if ((callbackResultItem.CallbackResult.SKDProgressCallback == null || !callbackResultItem.CallbackResult.SKDProgressCallback.IsCanceled) &&
							(!callbackResultItem.ClientUID.HasValue || callbackResultItem.ClientUID.Value == clientInfo.UID))
							result.Add(callbackResultItem.CallbackResult);
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

		public Guid? ClientUID { get; set; }
	}
}