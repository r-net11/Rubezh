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
				var minIndex = ClientsManager.ClientInfos.Any() ?
					ClientsManager.ClientInfos.Min(x => x.CallbackIndex) :
					0;
				CallbackResultItems.RemoveAll(x => x.CallbackResult.Index <= minIndex || (DateTime.Now - x.DateTime) > TimeSpan.FromDays(1));

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
					if (clientType.HasValue && (clientType.Value & clientInfo.ClientCredentials.ClientType) == ClientType.None)
						continue;
					if (clientUID.HasValue && clientUID.Value != clientInfo.UID)
						continue;
					if (layoutUIDs != null && !layoutUIDs.Contains(clientInfo.LayoutUID))
						continue;
					clientInfo.WaitEvent.Set();
				}
			}
		}

		public static PollResult Get(ClientInfo clientInfo)
		{
			if (clientInfo.IsDisconnecting)
				return GetDisconnecting();

			lock (CallbackResultItems)
			{
				var result = new PollResult();
				result.CallbackIndex = CallbackManager.Index;
				foreach (var callbackResultItem in CallbackResultItems)
				{
					if (callbackResultItem.CallbackResult.Index > clientInfo.CallbackIndex)
					{
						if (callbackResultItem.ClientType.HasValue && (callbackResultItem.ClientType.Value & clientInfo.ClientCredentials.ClientType) == ClientType.None)
							continue;
						if (callbackResultItem.ClientUID.HasValue && callbackResultItem.ClientUID.Value != clientInfo.UID)
							continue;
						if (callbackResultItem.LayoutUIDs != null && !callbackResultItem.LayoutUIDs.Contains(clientInfo.LayoutUID))
							continue;
						if (callbackResultItem.CallbackResult.GKProgressCallback != null && callbackResultItem.CallbackResult.GKProgressCallback.IsCanceled)
							continue;
						if (clientInfo.CallbackIndex < callbackResultItem.CallbackResult.Index)
							clientInfo.CallbackIndex = callbackResultItem.CallbackResult.Index;
						result.CallbackResults.Add(callbackResultItem.CallbackResult);
					}
				}
				return result;
			}
		}

		public static PollResult GetReconnectionRequired()
		{
			return new PollResult
			{
				CallbackIndex = CallbackManager.Index,
				IsReconnectionRequired = true
			};
		}

		public static PollResult GetDisconnecting()
		{
			return new PollResult
			{
				CallbackIndex = CallbackManager.Index,
				IsDisconnecting = true
			};
		}

		//public static PollResult GetConfigurationChanged()
		//{
		//	return new PollResult
		//	{
		//		CallbackIndex = CallbackManager.Index,
		//		IsConfigurationChanged = true
		//	};
		//}
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