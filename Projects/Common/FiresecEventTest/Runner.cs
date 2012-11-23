using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI;
using System.Threading;
using System.Diagnostics;

namespace FiresecEventTest
{
	public static class Runner
	{
		static NativeFiresecClient NativeFiresecClient;

		public static void Run()
		{
			var thread = new Thread(OnRun);
			thread.Start();
		}

		public static void OnRun()
		{
			NativeFiresecClient = new NativeFiresecClient();
			NativeFiresecClient.Connect("localhost", 211, "adm", "", true);
			NativeFiresecClient.NewJournalRecords += new Action<List<FiresecAPI.Models.JournalRecord>>(NativeFiresecClient_NewJournalRecords);

			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				NativeFiresecClient.CheckForRead();
			}
		}

		static void NativeFiresecClient_NewJournalRecords(List<FiresecAPI.Models.JournalRecord> journalRecords)
		{
			foreach(var journalRecord in journalRecords)
			{
				Trace.WriteLine(journalRecord.Description);
			}
		}
	}
}