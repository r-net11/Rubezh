using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FSAgentAPI;
using System.Diagnostics;

namespace FSAgentServer
{
	public class Watcher
	{
		NativeFiresecClient NativeFiresecClient;
		int LastJournalNo = 0;

		public Watcher(NativeFiresecClient nativeFiresecClient)
		{
			NativeFiresecClient = nativeFiresecClient;
			nativeFiresecClient.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
			nativeFiresecClient.StateChanged += new Action<string>(OnStateChanged);
			nativeFiresecClient.ParametersChanged += new Action<string>(OnParametersChanged);
			NativeFiresecClient.ProgressEvent += new Func<int, string, int, int, bool>(OnProgress);
		}

		OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
		{
			var resultData = new OperationResult<T>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (result.HasError == false)
				resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
			return resultData;
		}

		int TempOldCode = 0;

		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			foreach (var journalRecord in journalRecords)
			{
				//Trace.WriteLine("Callback journal no = " + journalRecord.OldId.ToString());
				if (TempOldCode > 0)
				{
					if (journalRecord.OldId - TempOldCode != 1)
					{
						Trace.WriteLine("Queue is not continuous " + journalRecord.OldId.ToString());
					}
				}
				TempOldCode = journalRecord.OldId;
			}

			CallbackManager.Add(new FSAgentCallbac() { JournalRecords = journalRecords });
		}

		void OnStateChanged(string deviceStates)
		{
			CallbackManager.Add(new FSAgentCallbac() { CoreCongig = deviceStates });
		}

		public void OnParametersChanged(string deviceParameters)
		{
			CallbackManager.Add(new FSAgentCallbac() { CoreDeviceParams = deviceParameters });
		}

		public event Func<int, string, int, int, bool> Progress;
		bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				return Progress(stage, comment, percentComplete, bytesRW);
			return true;
		}
	}
}