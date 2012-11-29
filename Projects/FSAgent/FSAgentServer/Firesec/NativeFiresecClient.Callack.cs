﻿using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;

namespace FSAgentServer
{
	public partial class NativeFiresecClient
	{
		public static bool IsSuspended { get; set; }
		int LastJournalNo = 0;
		static bool needToRead = false;
		static bool needToReadStates = false;
		static bool needToReadParameters = false;
		static bool needToReadJournal = false;

		string PrevCoreState = "";
		string PrevCoreDeviceParams = "";

        //static int count = 0;

		public void NewEventsAvailable(int eventMask)
		{
            //Trace.WriteLine("NewEventsAvailable " + count++.ToString());
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
						if (result != null && result.Result != null)
						{
							if (PrevCoreState != result.Result)
							{
								if (StateChanged != null)
									StateChanged(result.Result);
							}
							PrevCoreState = result.Result;
						}
						else
							App.Restart();
					}

					if (needToReadParameters)
					{
						needToReadParameters = false;
						var result = SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
						if (result != null && result.Result != null)
						{
							if (PrevCoreDeviceParams != result.Result)
							{
								if (ParametersChanged != null)
									ParametersChanged(result.Result);
							}
							PrevCoreDeviceParams = result.Result;
						}
						else
							App.Restart();
					}

					if (needToReadJournal)
					{
						needToReadJournal = false;
						var journalRecords = GetEventsFromLastId(LastJournalNo);

						if (NewJournalRecords != null)
							NewJournalRecords(journalRecords);
						else
							App.Restart();
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "NativeFiresecClient.NewEventsAvailable");
				}
				finally
				{
				}
			}
		}

		void SetLastEvent()
		{
			try
			{
				var result = ReadEvents(0, 100);
				if (result.HasError)
				{
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

				var result = ReadEvents(oldJournalNo, 100);
				if (result == null || result.HasError)
				{
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
							LastJournalNo = eventId;
							oldJournalNo = eventId;
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							journalRecords.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
				if (!hasNewRecords)
					break;
			}

			return journalRecords;
		}

		OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
		{
			var resultData = new OperationResult<T>();
			resultData.HasError = result.HasError;
			resultData.Error = result.Error;
			if (result.HasError == false)
				resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
			return resultData;
		}

		public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
		{
			if (IsSuspended)
				return true;

			try
			{
				if (ProgressEvent != null)
				{
					bool continueProgress = ContinueProgress;
					ContinueProgress = true;
					ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
					return continueProgress;
				}
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
				return false;
			}
		}

		public static bool ContinueProgress = true;

		public event Action<List<JournalRecord>> NewJournalRecords;
		public event Action<string> StateChanged;
		public event Action<string> ParametersChanged;
		public event Func<int, string, int, int, bool> ProgressEvent;
	}
}