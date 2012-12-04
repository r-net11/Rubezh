using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;
using FSAgentAPI;
using Infrastructure.Common.BalloonTrayTip;

namespace FSAgentServer
{
	public partial class NativeFiresecClient
	{
		public bool IsPing { get; set; }
		public static bool IsSuspended { get; set; }
		int LastJournalNo = 0;
		static bool needToRead = false;
		static bool needToReadStates = false;
		static bool needToReadParameters = false;
		static bool needToReadJournal = false;

		string PrevCoreState = "";
		string PrevCoreDeviceParams = "";

		public void NewEventsAvailable(int eventMask)
		{
            if (IsPing)
			{
				needToRead = true;
				needToReadJournal = ((eventMask & 1) == 1);
				needToReadStates = ((eventMask & 2) == 2);
				needToReadParameters = ((eventMask & 8) == 8);
				WatcherManager.WaikeOnEvent();
			}
		}

		public void CheckForRead(bool force = false)
		{
			if (force)
			{
				needToRead = true;
				needToReadStates = true;
				needToReadParameters = true;
				needToReadJournal = true;
			}

			if (IsSuspended)
				return;

			if (needToRead)
			{
				needToRead = false;

				try
				{
					if (needToReadStates)
					{
						needToReadStates = false;
						var result = SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
						if (result != null && result.Result != null && result.HasError == false)
						{
							if (PrevCoreState != result.Result)
							{
								CallbackManager.Add(new FSAgentCallbac() { CoreCongig = result.Result });
							}
							PrevCoreState = result.Result;
						}
						else
						{
							OnCriticalError();
						}
					}

					if (needToReadParameters)
					{
						needToReadParameters = false;
						var result = SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
						if (result != null && result.Result != null && result.HasError == false)
						{
							if (PrevCoreDeviceParams != result.Result)
							{
								CallbackManager.Add(new FSAgentCallbac() { CoreDeviceParams = result.Result });
							}
							PrevCoreDeviceParams = result.Result;
						}
						else
						{
							OnCriticalError();
						}
					}

					if (needToReadJournal)
					{
						needToReadJournal = false;
						var journalRecords = GetEventsFromLastId(LastJournalNo);

						if (journalRecords != null)
						{
							CallbackManager.Add(new FSAgentCallbac() { JournalRecords = journalRecords });
						}
						else
						{
							OnCriticalError();
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "NativeFiresecClient.NewEventsAvailable");
				}
			}
		}

		void SetLastEvent()
		{
			try
			{
				var result = ReadEvents(0, 100);
				if (result == null || result.Result == null || result.HasError)
				{
					OnCriticalError();
					LastJournalNo = 0;
				}
				var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result.Result);
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var journalItem in document.Journal)
					{
						var intValue = int.Parse(journalItem.IDEvents);
						if (intValue > LastJournalNo)
							LastJournalNo = intValue;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "NativeFiresecClient.SetLastEvent");
			}
		}

		List<JournalRecord> GetEventsFromLastId(int oldJournalNo)
		{
			var journalRecords = new List<JournalRecord>();

			var hasNewRecords = true;
			for (int i = 0; i < 100; i++)
			{
				hasNewRecords = false;

				var result = ReadEvents(LastJournalNo, 100);
				if (result == null || result.Result == null || result.HasError)
				{
					OnCriticalError();
					return new List<JournalRecord>();
				}

				var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result.Result);
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var innerJournalItem in document.Journal)
					{
						var eventId = int.Parse(innerJournalItem.IDEvents);
						if (eventId > oldJournalNo)
						{
							if (eventId > LastJournalNo)
							{
								LastJournalNo = eventId;
							}
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							journalRecords.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
				if (!hasNewRecords)
					break;
			}

			if (journalRecords.Count > 0)
				journalRecords = journalRecords.OrderBy(x => x.OldId).ToList();
			return journalRecords;
		}

		public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (IsSuspended)
				return true;

            try
            {
                bool continueProgress = ContinueProgress;
                ContinueProgress = true;
				var fsProgressInfo = new FSProgressInfo()
				{
					Stage = stage,
					Comment = comment,
					PercentComplete = percentComplete,
					BytesRW = bytesRW
				};
				CallbackManager.Add(new FSAgentCallbac() { FSProgressInfo = fsProgressInfo });
                return continueProgress;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
                return false;
            }
		}

		public static bool ContinueProgress = true;

		void OnCriticalError()
		{
            BalloonHelper.ShowWarning("Агент Firesec", "Потеря соединения с драйвером");
			CallbackManager.SetConnectionLost(true);
			var result = Connect();
			if (result == null || result.Result == false || result.HasError)
			{
				App.Restart();
			}
			else
			{
				CallbackManager.SetConnectionLost(false);
			}
		}
	}
}