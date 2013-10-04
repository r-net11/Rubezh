using System;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure.Common;

namespace GKProcessor
{
	public partial class Watcher
	{
		public bool IsStopping = false;
		AutoResetEvent StopEvent;
		Thread RunThread;
		public GkDatabase GkDatabase { get; private set; }
		public DateTime LastUpdateTime { get; private set; }
		DateTime LastLicenseCheckTime;
		bool HasLicense = true;

		public Watcher(GkDatabase gkDatabase)
		{
			GkDatabase = gkDatabase;
		}

		public void StartThread()
		{
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
				RunThread.Start();
			}
		}

		public void StopThread()
		{
			IsStopping = true;

			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (RunThread != null)
			{
				RunThread.Join(TimeSpan.FromSeconds(1));
			}
			RunThread = null;
		}

		void OnRunThread()
		{
			try
			{
				GetAllStates();
				if (!IsAnyDBMissmatch)
				{
					ReadMissingJournalItems();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalWatcher.OnRunThread GetAllStates");
			}

			while (true)
			{
				if (!IsAnyDBMissmatch)
				{
					if ((DateTime.Now - LastLicenseCheckTime).TotalMinutes > 1000000)
					{
						var hasLicense = LicenseHelper.CheckLicense(false);
						if (hasLicense != HasLicense)
						{
							HasLicense = hasLicense;
							foreach (var binaryObject in GkDatabase.BinaryObjects)
							{
								var baseState = binaryObject.BinaryBase.GetXBaseState();
								baseState.IsNoLicense = !hasLicense;
							}
						}

						if (hasLicense)
						{
							LastLicenseCheckTime = DateTime.Now;
						}
						else
						{
							LastUpdateTime = DateTime.Now;
							Thread.Sleep(TimeSpan.FromSeconds(10));
						}
					}

					try
					{
						CheckTasks();
					}
					catch (Exception e)
					{
						Logger.Error(e, "JournalWatcher.OnRunThread CheckTasks");
					}

					try
					{
						CheckDelays();
					}
					catch (Exception e)
					{
						Logger.Error(e, "JournalWatcher.OnRunThread CheckNPT");
					}

					try
					{
						PingJournal();
					}
					catch (Exception e)
					{
						Logger.Error(e, "JournalWatcher.OnRunThread PingJournal");
					}

					try
					{
						PingNextState();
					}
					catch (Exception e)
					{
						Logger.Error(e, "JournalWatcher.OnRunThread PingNextState");
					}
				}

				if (StopEvent != null)
				{
					if (StopEvent.WaitOne(10))
						break;
				}

				LastUpdateTime = DateTime.Now;
			}
		}
	}
}