using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<CallbackResult> Poll(Guid uid)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
			if (clientInfo != null)
			{
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

		public static void NotifySKDObjectStateChanged(SKDCallbackResult skdCallbackResult)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.SKDObjectStateChanged,
				SKDCallbackResult = skdCallbackResult
			};
			CallbackManager.Add(callbackResult);
		}

		public void NotifyNewJournal(List<JournalRecord> journalRecords)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.NewEvents,
				JournalRecords = journalRecords
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyArchivePortionCompleted(List<JournalRecord> journalRecords)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.ArchiveCompleted,
				JournalRecords = journalRecords
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifyGKArchiveCompleted(List<JournalItem> journallItems, Guid archivePortionUID)
		{
			var callbackResult = new CallbackResult()
			{
				ArchivePortionUID = archivePortionUID,
				CallbackResultType = CallbackResultType.GKArchiveCompleted,
				JournalItems = journallItems
			};
			CallbackManager.Add(callbackResult);
		}

		public static void NotifySKDArchiveCompleted(List<SKDJournalItem> journallItems)
		{
			var callbackResult = new CallbackResult()
			{
				CallbackResultType = CallbackResultType.SKDArchiveCompleted,
				SKDCallbackResult = new SKDCallbackResult()
				{
					JournalItems = journallItems
				}
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
	}
}