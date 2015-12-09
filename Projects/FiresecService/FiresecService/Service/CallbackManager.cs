using RubezhAPI;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		static List<CallbackResultItem> CallbackResultItems = new List<CallbackResultItem>();
		public static int Index { get; private set; }

		public static void Add(CallbackResult callbackResult, ClientType? clientType = null, Guid? clientUID = null, IEnumerable<Guid> layoutUIDs = null)
		{
			lock (CallbackResultItems)
			{
				CallbackResultItems.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

				callbackResult.Index = ++Index;
				var newCallbackResultItem = new CallbackResultItem()
				{
					CallbackResult = callbackResult,
					DateTime = DateTime.Now,
					ClientType = clientType,
					ClientUID = clientUID,
					LayoutUIDs = layoutUIDs
				};
				CallbackResultItems.Add(newCallbackResultItem);

				if (callbackResult.CallbackResultType == CallbackResultType.GKProgress)
				{
					var callbackResultItem = CallbackResultItems.FirstOrDefault(x => x.CallbackResult.GKProgressCallback != null && x.CallbackResult.GKProgressCallback.UID == callbackResult.GKProgressCallback.UID);
					if (callbackResultItem != null)
					{
						if (callbackResult.GKProgressCallback.LastActiveDateTime > callbackResultItem.CallbackResult.GKProgressCallback.LastActiveDateTime)
						{
							CallbackResultItems.Remove(callbackResultItem);
						}
					}
				}

				foreach (var clientInfo in ClientsManager.ClientInfos)
				{
					if (clientType.HasValue && clientType.Value != clientInfo.ClientCredentials.ClientType)
						continue;
					if (clientUID.HasValue && clientUID.Value != clientInfo.UID)
						continue;
					if (layoutUIDs != null && !layoutUIDs.Contains(clientInfo.LayoutUID))
						continue;
					clientInfo.WaitEvent.Set();
				}
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
					if (callbackResultItem.CallbackResult.Index > clientInfo.CallbackIndex)
					{
						if (callbackResultItem.ClientType.HasValue && callbackResultItem.ClientType.Value != clientInfo.ClientCredentials.ClientType)
							continue;
						if (callbackResultItem.ClientUID.HasValue && callbackResultItem.ClientUID.Value != clientInfo.UID)
							continue;
						if (callbackResultItem.LayoutUIDs != null && !callbackResultItem.LayoutUIDs.Contains(clientInfo.LayoutUID))
							continue;
						if (callbackResultItem.CallbackResult.GKProgressCallback != null && callbackResultItem.CallbackResult.GKProgressCallback.IsCanceled)
							continue;
						result.Add(callbackResultItem.CallbackResult);
					}
				}
				return result;
			}
		}
	}

	public class CallbackResultItem
	{
		public CallbackResult CallbackResult { get; set; }
		public DateTime DateTime { get; set; }
		public ClientType? ClientType { get; set; }
		public Guid? ClientUID { get; set; }
		public IEnumerable<Guid> LayoutUIDs { get; set; }
	}
}