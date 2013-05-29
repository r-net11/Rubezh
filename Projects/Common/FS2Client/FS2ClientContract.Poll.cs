using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common;
using FS2Api;

namespace FS2Client
{
	public partial class FS2ClientContract
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
			try
			{
				IsClosing = true;
				if (PollThread != null)
				{
					CancelPoll(FS2Factory.UID);
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
			catch (Exception e)
			{
				Logger.Error(e, ".StopPollThread");
			}
			finally
			{
				IsClosing = false;
			}
		}

		void OnPoll()
		{
			while (true)
			{
				try
				{
					if (IsClosing || IsDisconnecting)
						return;

					if (SuspendPoll)
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
					}

					IsOperationByisy = true;
					StartOperationDateTime = DateTime.Now;
					CircleDateTime = DateTime.Now;

					var changeResults = Poll(FS2Factory.UID);
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

							if (changeResult.FS2ProgressInfo != null)
							{
								if (Progress != null)
									Progress(changeResult.FS2ProgressInfo);
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
				catch (ThreadAbortException) { }
				catch (Exception e)
				{
					Logger.Error(e, "FS2Client.OnPoll");
				}
				finally
				{
					IsOperationByisy = false;
				}
			}
		}

		public event Action<string> CoreConfigChanged;
		public event Action<string> CoreDeviceParamsChanged;
		public event Action<List<FS2JournalItem>> NewJournalRecords;
		public event Action<FS2ProgressInfo> Progress;
	}
}