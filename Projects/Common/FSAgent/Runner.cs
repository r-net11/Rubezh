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
		//static NativeFiresecClient NativeFiresecClient;
        static FiresecSerializedClient FiresecSerializedClient;
        static Watcher Watcher;

		public static void Run()
		{
			var thread = new Thread(OnRun);
			thread.Start();
		}

		static void OnRun()
		{
			try
			{
                FiresecSerializedClient = new FiresecSerializedClient();
                FiresecSerializedClient.Connect("localhost", 211, "adm", "", true);
                Watcher = new Watcher(FiresecSerializedClient, true, true);
                Watcher.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
                Watcher.DevicesStateChanged += new Action<List<DeviceState>>(OnDevicesStateChanged);
                Watcher.DevicesParametersChanged += new Action<List<DeviceState>>(OnDevicesParametersChanged);
                Watcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZonesStateChanged);
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
                    FiresecSerializedClient.NativeFiresecClient.CheckForRead();
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

        static void OnDevicesStateChanged(List<DeviceState> deviceStates)
        {
            Trace.WriteLine("OnDevicesStateChanged");
        }

        static void OnDevicesParametersChanged(List<DeviceState> deviceStates)
        {
            Trace.WriteLine("OnDevicesParametersChanged");
        }

        static void OnZonesStateChanged(List<ZoneState> zoneStates)
        {
            Trace.WriteLine("OnZonesStateChanged");
        }
	}
}