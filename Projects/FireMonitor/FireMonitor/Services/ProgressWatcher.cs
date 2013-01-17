using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using FireMonitor.ViewModels;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace FireMonitor
{
	public static class ProgressWatcher
	{
		static ProgressViewModel progressViewModel = new ProgressViewModel();
		static DispatcherTimer ClosingTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };

		public static void Run()
		{
			if (FiresecManager.FiresecDriver != null && FiresecManager.FiresecDriver.Watcher != null)
			{
				ClosingTimer.Tick += new EventHandler(ClosingTimer_Tick);
				FiresecManager.FiresecDriver.Watcher.Progress += new Action<int, string, int, int>(Watcher_Progress);
			}
		}

        public static void Close()
        {
            progressViewModel.Close();
        }

		static void Watcher_Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);

			SafeCall(() =>
			{
				if (FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice.DeviceState.StateType == StateType.Unknown)
				{
					progressViewModel.Add(comment);
					progressViewModel.SelectedProgressItem = progressViewModel.ProgressItems.LastOrDefault();
					if (!progressViewModel.IsShown)
					{
						DialogService.ShowWindow(progressViewModel);
						progressViewModel.IsShown = true;
						ClosingTimer.Start();
					}
				}
			});
		}

		static void ClosingTimer_Tick(object sender, EventArgs e)
		{
			OnDevicesStateChanged(null);
		}

		static void OnDevicesStateChanged(object obj)
		{
			if (FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice.DeviceState.StateType != StateType.Unknown)
			{
				ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
				ClosingTimer.Stop();
				progressViewModel.Close();
			}
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}