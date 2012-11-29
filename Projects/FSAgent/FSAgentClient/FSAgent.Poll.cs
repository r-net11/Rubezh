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
		Thread PollThread;
		bool IsClosing = false;

		public void Start()
		{
			StartPollThread();
			StartLifetime();
		}

		public void Stop()
		{
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
				IsOperationByisy = true;
				StartOperationDateTime = DateTime.Now;
				try
				{
					if (IsClosing)
						return;

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