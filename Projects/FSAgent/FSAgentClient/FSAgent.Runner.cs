using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI.Models;
using FSAgentAPI;
using System.Diagnostics;

namespace FSAgentClient
{
	public partial class FSAgent
	{
		Thread RunThread;
		bool IsClosing = false;

		public void Start()
		{
			RunThread = new Thread(OnRun);
			RunThread.Start();
		}

		public void Stop()
		{
			if (RunThread != null)
			{
				IsClosing = true;
				RunThread.Join(TimeSpan.FromSeconds(5));
			}
		}

		void OnRun()
		{
			while (true)
			{
				if (IsClosing)
					return;

				var changeResults = Poll(FSAgentFactory.UID);
				if (changeResults != null)
				{
					foreach (var changeResult in changeResults)
					{
						if (changeResult.CoreCongig != null)
							OnCoreConfigChanged(changeResult.CoreCongig);

						if (changeResult.CoreDeviceParams != null)
							OnCoreDeviceParamsChanged(changeResult.CoreDeviceParams);

						if (changeResult.JournalRecords != null && changeResult.JournalRecords.Count > 0)
							OnNewJournalRecords(changeResult.JournalRecords);

						if (changeResult.FSProgressInfo != null)
							OnProgress(changeResult.FSProgressInfo);
					}
				}
			}
		}

		public event Action<string> CoreConfigChanged;
		void OnCoreConfigChanged(string coreConfig)
		{
			if (CoreConfigChanged != null)
				CoreConfigChanged(coreConfig);
		}

		public event Action<string> CoreDeviceParamsChanged;
		void OnCoreDeviceParamsChanged(string coreDeviceParams)
		{
			if (CoreDeviceParamsChanged != null)
				CoreDeviceParamsChanged(coreDeviceParams);
		}

		public event Action<List<JournalRecord>> NewJournalRecords;
		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			if (NewJournalRecords != null)
				NewJournalRecords(journalRecords);
		}

		public event Action<FSProgressInfo> Progress;
		void OnProgress(FSProgressInfo deviceStates)
		{
			if (Progress != null)
				Progress(deviceStates);
		}
	}
}