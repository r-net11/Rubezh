using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RubezhAPI;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System.IO;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<CallbackResult> Poll(Guid uid)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
			if (clientInfo != null)
			{
                clientInfo.LastPollDateTime = DateTime.Now;
				var result = CallbackManager.Get(clientInfo);
				if (result.Count == 0)
				{
					clientInfo.WaitEvent = new AutoResetEvent(false);
					if (clientInfo.WaitEvent.WaitOne(TimeSpan.FromMinutes(5)))
					{
						result = CallbackManager.Get(clientInfo);
					}
				}
				return result;
			}
			return new List<CallbackResult>();
		}

		public static void NotifyGKProgress(GKProgressCallback gkProgressCallback)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.GKProgress,
				GKProgressCallback = gkProgressCallback
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyGKObjectStateChanged(GKCallbackResult gkCallbackResult)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.GKObjectStateChanged,
				GKCallbackResult = gkCallbackResult
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyAutomation(AutomationCallbackResult automationCallbackResult, Guid? clientUID = null)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.AutomationCallbackResult,
				AutomationCallbackResult = automationCallbackResult,
			};
			CallbackManager.Add(callbackResult, clientUID);
		}

		public static void NotifyJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = isNew ? CallbackResultType.NewEvents : CallbackResultType.UpdateEvents,
				JournalItems = journalItems
			};
			CallbackManager.Add(callbackResult);
		}

        public static void NotifyConfigurationChanged()
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.ConfigurationChanged
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyGKParameterChanged(Guid objectUID, List<GKProperty> deviceProperties)
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
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyOperationResult_GetAllUsers(OperationResult<List<GKUser>> result, bool isGk, Guid deviceUID)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.OperationResult,
				CallbackOperationResult = new CallbackOperationResult()
				{
					CallbackOperationResultType = isGk ? CallbackOperationResultType.GetGKUsers : CallbackOperationResultType.GetPmfUsers,
					Error = result.Error,
					HasError = result.HasError,
					Users = result.Result, 
					DeviceUID = deviceUID
				}
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyOperationResult_RewriteUsers(OperationResult<bool> result)
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
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyOperationResult_WriteConfiguration(OperationResult<bool> result)
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
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyOperationResult_ReadConfigurationFromGKFile(OperationResult<string> result)
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
			CallbackManager.Add(callbackResult);
		}
	}
}