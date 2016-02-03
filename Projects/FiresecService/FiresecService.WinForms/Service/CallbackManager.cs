using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.AutomationCallback;
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

		public static void Add(CallbackResult callbackResult, ClientType? clientType = null, Guid? clientUID = null)
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
					ClientUID = clientUID
				};
				CallbackResultItems.Add(newCallbackResultItem);

				if (callbackResult.CallbackResultType == CallbackResultType.GKProgress)
				{
					var callbackResultItem = CallbackResultItems.FirstOrDefault(x => x.CallbackResult.GKProgressCallback != null && x.CallbackResult.GKProgressCallback.UID == callbackResult.GKProgressCallback.UID);
					if (callbackResultItem != null && callbackResult.GKProgressCallback.LastActiveDateTime > callbackResultItem.CallbackResult.GKProgressCallback.LastActiveDateTime)
						CallbackResultItems.Remove(callbackResultItem);
				}

				if (callbackResult.AutomationCallbackResult != null && callbackResult.AutomationCallbackResult.Data is PlanCallbackData)
				{
					var callbackResultData = callbackResult.AutomationCallbackResult.Data as PlanCallbackData;
					foreach (var callbackResultItem in CallbackResultItems)
						if (callbackResultItem.CallbackResult.AutomationCallbackResult != null && callbackResultItem.CallbackResult.AutomationCallbackResult.Data is PlanCallbackData)
						{
							var callbackResuultItemData = callbackResultItem.CallbackResult.AutomationCallbackResult.Data as PlanCallbackData;
							if (callbackResultData.ElementUid == callbackResuultItemData.ElementUid &&
								callbackResultData.ElementPropertyType == callbackResuultItemData.ElementPropertyType &&
								callbackResultData.ElementUid == callbackResuultItemData.ElementUid &&
								callbackResult.AutomationCallbackResult.CallbackUID != callbackResultItem.CallbackResult.AutomationCallbackResult.CallbackUID)
							{
								CallbackResultItems.Remove(callbackResultItem);
								break;
							}
						};
				}

				foreach (var clientInfo in ClientsManager.ClientInfos)
				{
					if (clientType.HasValue && (clientType.Value & clientInfo.ClientCredentials.ClientType) == ClientType.None)
						continue;
					if (clientUID.HasValue && clientUID.Value != clientInfo.UID)
						continue;
					if (!AutomationHelper.CheckLayoutFilter(callbackResult.AutomationCallbackResult, clientInfo.LayoutUID))
						continue;
					clientInfo.WaitEvent.Set();
				}
			}
		}

		public static PollResult Get(ClientInfo clientInfo)
		{
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
						if (callbackResultItem.CallbackResult.GKProgressCallback != null && callbackResultItem.CallbackResult.GKProgressCallback.IsCanceled)
							continue;
						if (!AutomationHelper.CheckLayoutFilter(callbackResultItem.CallbackResult.AutomationCallbackResult, clientInfo.LayoutUID))
							continue;
						if (callbackResultItem.CallbackResult.AutomationCallbackResult != null && callbackResultItem.CallbackResult.AutomationCallbackResult.Data is GlobalVariableCallBackData)
						{
							var data = callbackResultItem.CallbackResult.AutomationCallbackResult.Data as GlobalVariableCallBackData;
							if (data.InitialClientUID.HasValue && data.InitialClientUID.Value == clientInfo.UID)
								continue;
						}
						if (clientInfo.CallbackIndex < callbackResultItem.CallbackResult.Index)
							clientInfo.CallbackIndex = callbackResultItem.CallbackResult.Index;
						result.CallbackResults.Add(callbackResultItem.CallbackResult);
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
	}
}