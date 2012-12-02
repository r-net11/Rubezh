using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI.Models;
using FSAgentAPI;
using System.Diagnostics;
using Common;

namespace FSAgentClient
{
	public partial class FSAgent
	{
		public bool SuspendPoll = false;
		Thread PollThread;
		bool IsClosing = false;
        bool IsDisconnecting = false;

		public void Start()
		{
			StartPollThread();
			StartLifetime();
		}

		public void Stop()
		{
            IsDisconnecting = true;
			StopLifetime();
			StopPollThread();
		}

		public void StartPollThread()
		{
			PollThread = new Thread(OnPoll);
			PollThread.Start();
		}

		public void StopPollThread()
		{
			if (PollThread != null)
			{
				IsClosing = true;
				if (!PollThread.Join(TimeSpan.FromSeconds(5)))
				{
					try
					{
						PollThread.Abort();
					}
					catch { }
				}
			}
		}

		void OnPoll()
		{
			while (true)
			{
				try
				{
					if (IsClosing)
						return;

					if (SuspendPoll)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
					}

					IsOperationByisy = true;
					StartOperationDateTime = DateTime.Now;

					var changeResults = Poll(FSAgentFactory.UID);
					if (changeResults != null)
					{
						foreach (var changeResult in changeResults)
						{
							if (changeResult.CoreCongig != null)
							{
								if (CoreConfigChanged != null)
									CoreConfigChanged(changeResult.CoreCongig);
							}

							if (changeResult.CoreDeviceParams != null)
							{
								if (CoreDeviceParamsChanged != null)
									CoreDeviceParamsChanged(changeResult.CoreDeviceParams);
							}

							if (changeResult.JournalRecords != null && changeResult.JournalRecords.Count > 0)
							{
								if (NewJournalRecords != null)
									NewJournalRecords(changeResult.JournalRecords);
							}

							if (changeResult.FSProgressInfo != null)
							{
								if (Progress != null)
									Progress(changeResult.FSProgressInfo);
							}
							if (changeResult.IsConnectionLost)
							{
								if (ConnectionLost != null)
									ConnectionLost();
							}
							else
							{
								if (ConnectionAppeared != null)
									ConnectionAppeared();
							}
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "FSAgentClient.OnRun");
				}
				finally
				{
					IsOperationByisy = false;
				}
			}
		}

		public event Action<string> CoreConfigChanged;
		public event Action<string> CoreDeviceParamsChanged;
		public event Action<List<JournalRecord>> NewJournalRecords;
		public event Action<FSProgressInfo> Progress;
	}
}