using RubezhAPI;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public PollResult Poll(Guid clientUID, int callbackIndex)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
			if (clientInfo != null)
			{
				clientInfo.LastPollDateTime = DateTime.Now;
				if (clientInfo.CallbackIndex > callbackIndex && callbackIndex != -1)
					clientInfo.CallbackIndex = callbackIndex;
				global::FiresecService.ViewModels.MainViewModel.Current.OnPoll(clientUID);
				var result = CallbackManager.Get(clientInfo);
				if (result.CallbackResults.Count == 0)
				{
					clientInfo.WaitEvent = new AutoResetEvent(false);
					if (clientInfo.WaitEvent.WaitOne(TimeSpan.FromMinutes(1)))
					{
						result = CallbackManager.Get(clientInfo);
					}
				}
				return result;
			}
			global::FiresecService.ViewModels.MainViewModel.Current.OnPoll(clientUID);
			return CallbackManager.GetReconnectionRequired();
		}

		public static void NotifyGKProgress(GKProgressCallback gkProgressCallback, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.GKProgress,
				GKProgressCallback = gkProgressCallback
			};
			ClientType? clientType = null;
			if (gkProgressCallback != null)
				clientType = gkProgressCallback.GKProgressClientType == GKProgressClientType.Administrator ?
					ClientType.Administrator :
					ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other;
			CallbackManager.Add(callbackResult, clientType, clientUID);
		}

		public static void NotifyGKObjectStateChanged(GKCallbackResult gkCallbackResult)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.GKObjectStateChanged,
				GKCallbackResult = gkCallbackResult
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other);
		}

		public static void NotifyAutomation(AutomationCallbackResult automationCallbackResult, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.AutomationCallbackResult,
				AutomationCallbackResult = automationCallbackResult,
			};
			var layoutUIDs = automationCallbackResult != null && automationCallbackResult.Data != null && automationCallbackResult.Data.LayoutFilter != null ?
				automationCallbackResult.Data.LayoutFilter.LayoutsUIDs :
				null;
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other, clientUID, layoutUIDs);
		}

		public static void NotifyJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = isNew ? CallbackResultType.NewEvents : CallbackResultType.UpdateEvents,
				JournalItems = journalItems
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other);
		}

		public static void NotifyConfigurationChanged()
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.ConfigurationChanged
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other);
		}

		public static void NotifyGKParameterChanged(Guid objectUID, List<GKProperty> deviceProperties, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.GKPropertyChanged,
				GKPropertyChangedCallback = new GKPropertyChangedCallback()
				{
					ObjectUID = objectUID,
					DeviceProperties = deviceProperties
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other);
		}

		public static void NotifyOperationResult_GetAllUsers(OperationResult<List<GKUser>> result, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult()
				{
					CallbackOperationResultType = CallbackOperationResultType.GetAllUsers,
					Error = result.Error,
					HasError = result.HasError,
					Users = result.Result
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Administrator, clientUID);
		}

		public static void NotifyOperationResult_RewriteUsers(OperationResult<bool> result, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult()
				{
					CallbackOperationResultType = CallbackOperationResultType.RewriteUsers,
					Error = result.Error,
					HasError = result.HasError,
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Administrator, clientUID);
		}

		public static void NotifyOperationResult_WriteConfiguration(OperationResult<bool> result, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult()
				{
					CallbackOperationResultType = CallbackOperationResultType.WriteConfiguration,
					Error = result.Error,
					HasError = result.HasError,
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Administrator, clientUID);
		}

		public static void NotifyOperationResult_ReadConfigurationFromGKFile(OperationResult<string> result, Guid? clientUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult()
				{
					CallbackOperationResultType = CallbackOperationResultType.ReadConfigurationFromGKFile,
					Error = result.Error,
					HasError = result.HasError,
					FileName = result.Result
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Administrator, clientUID);
		}

		public static void NotifyOperationResult_GetArchivePage(OperationResult<List<JournalItem>> result, Guid clientUid)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult
				{
					CallbackOperationResultType = CallbackOperationResultType.GetArchivePage,
					Error = result.Error,
					HasError = result.HasError,
					JournalItems = result.Result,
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other, clientUid);
		}

		public static void NotifyOperationResult_GetJournal(OperationResult<List<JournalItem>> result, Guid clientUid, Guid journalClientUid)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult
				{
					CallbackOperationResultType = CallbackOperationResultType.GetJournal,
					Error = result.Error,
					HasError = result.HasError,
					JournalItems = result.Result,
					ClientUid = journalClientUid,
				}
			};
			CallbackManager.Add(callbackResult, ClientType.Monitor | ClientType.OPC | ClientType.WebService | ClientType.Other, clientUid);
		}
	}
}