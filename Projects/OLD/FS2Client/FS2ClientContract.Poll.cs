using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI.Models;
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
							if (changeResult.ChangedDeviceStates != null && changeResult.ChangedDeviceStates.Count > 0)
							{
								OnDeviceStateChanged(changeResult.ChangedDeviceStates);
							}

							if (changeResult.ChangedDeviceParameters != null && changeResult.ChangedDeviceParameters.Count > 0)
							{
								OnDeviceParametersChanged(changeResult.ChangedDeviceParameters);
							}

							if (changeResult.ChangedZoneStates != null && changeResult.ChangedZoneStates.Count > 0)
							{
								OnZoneStatesChanged(changeResult.ChangedZoneStates);
							}

							if (changeResult.JournalItems != null && changeResult.JournalItems.Count > 0)
							{
								if (NewJournalItems != null)
									NewJournalItems(changeResult.JournalItems);
							}

							if (changeResult.ArchiveJournalItems != null && changeResult.ArchiveJournalItems.Count > 0)
							{
								if (NewArchiveJournalItems != null)
									NewArchiveJournalItems(changeResult.ArchiveJournalItems);
							}

							if (changeResult.FS2ProgressInfo != null)
							{
								if (Progress != null)
									Progress(changeResult.FS2ProgressInfo);
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

		void OnDeviceStateChanged(List<DeviceState> deviceStates)
		{
			if (DeviceStateChanged != null)
				DeviceStateChanged(deviceStates);
		}

		void OnDeviceParametersChanged(List<DeviceState> deviceStates)
		{
			if (DeviceParametersChanged != null)
				DeviceParametersChanged(deviceStates);
		}

		void OnZoneStatesChanged(List<ZoneState> zoneStates)
		{
			if (ZoneStatesChanged != null)
				ZoneStatesChanged(zoneStates);
		}

		public event Action<List<DeviceState>> DeviceStateChanged;
		public event Action<List<DeviceState>> DeviceParametersChanged;
		public event Action<List<ZoneState>> ZoneStatesChanged;
		public event Action<List<FS2JournalItem>> NewJournalItems;
		public event Action<List<FS2JournalItem>> NewArchiveJournalItems;
		public event Action<FS2ProgressInfo> Progress;
	}
}