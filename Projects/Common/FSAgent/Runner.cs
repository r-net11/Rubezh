using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI;
using System.Threading;
using System.Diagnostics;
using FiresecAPI.Models;
using System.Windows.Threading;
using Common;

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

		static void OnRun()
		{
			try
			{
				NativeFiresecClient = new NativeFiresecClient();
				NativeFiresecClient.Connect("localhost", 211, "adm", "", true);
				NativeFiresecClient.NewJournalRecords += new Action<List<FiresecAPI.Models.JournalRecord>>(OnNewJournalRecords);
			}
			catch (Exception e)
			{
				Logger.Error(e, "OnRun");
			}

			while (true)
			{
				try
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
					NativeFiresecClient.CheckForRead();
				}
				catch (Exception e)
				{
					Logger.Error(e, "OnRun.while");
				}
			}
		}

		static void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			try
			{
				foreach (var journalRecord in journalRecords)
				{
					//MainWindow.Current.Dispatcher.Invoke(new Action(() =>
					//{
					//    MainWindow.Current.JournalItems.Insert(0, journalRecord.SystemTime.ToString() + " - " + journalRecord.Description);
					//    ;
					//}));
					Trace.WriteLine(journalRecord.Description);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "OnRun.OnNewJournalRecords");
			}
		}
	}
}