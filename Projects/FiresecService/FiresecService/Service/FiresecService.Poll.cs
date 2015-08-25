using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.AutomationCallback;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
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

		public static void NotifyNewJournalItems(List<JournalItem> journalItems)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.NewEvents,
				JournalItems = journalItems
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyArchiveCompleted(List<JournalItem> journallItems, Guid archivePortionUID)
		{
			var callbackResult = new CallbackResult()
			{
				ArchivePortionUID = archivePortionUID,
				CallbackResultType = CallbackResultType.ArchiveCompleted,
				JournalItems = journallItems,
			};
			CallbackManager.Add(callbackResult);
		}

        public static void NotifyDbCompleted(DbCallbackResult dbCallbackResult)
        {
            var callbackResult = new CallbackResult()
            {
                DbCallbackResult = dbCallbackResult,
                CallbackResultType = CallbackResultType.QueryDb,
            };
            CallbackManager.Add(callbackResult);
        }

		public void NotifyConfigurationChanged()
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

		public static void NotifyOperationResult_GetAllUsers(OperationResult<List<GKUser>> result)
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