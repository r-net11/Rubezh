using System;
using Common;
using FiresecDB;
using FSAgentAPI;
using Infrastructure.Common.Windows.BalloonTrayTip;

namespace FSAgentServer
{
	public partial class NativeFiresecClient
	{
		public bool IsPing { get; set; }

		public static bool IsSuspended { get; set; }

		static bool needToRead = false;
		static bool needToReadStates = false;
		static bool needToReadParameters = false;
		static bool needToReadJournal = false;
		static int CriticalErrorsCount = 0;
		public static bool ContinueProgress = true;

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
							CallbackManager.Add(new FSAgentCallbac() { CoreCongig = result.Result });
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
							CallbackManager.Add(new FSAgentCallbac() { CoreDeviceParams = result.Result });
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
							var addedJournalRecords = DatabaseHelper.AddJournalRecords(journalRecords);
							CallbackManager.Add(new FSAgentCallbac() { JournalRecords = addedJournalRecords });
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

		private void OnCriticalError()
		{
			CriticalErrorsCount++;
			if (CriticalErrorsCount > 5)
				return;

			BalloonHelper.ShowFromAgent("Потеря соединения с драйвером");
			CallbackManager.SetConnectionLost(true);
			var result = Connect();
			if (result == null || result.Result == false || result.HasError)
			{
				App.Restart();
				return;
			}
			else
			{
				CallbackManager.SetConnectionLost(false);
			}

			if (CriticalErrorsCount > 2)
				App.Restart();
		}
	}
}