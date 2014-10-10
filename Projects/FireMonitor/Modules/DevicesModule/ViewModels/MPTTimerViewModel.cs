using System;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class MPTTimerViewModel : DialogViewModel, IWindowIdentity
	{
		public Device Device { get; private set; }
		Guid _guid;
		DispatcherTimer DispatcherTimer;

		public MPTTimerViewModel(Device device)
		{
			Title = "Включение МПТ. Обратный отсчет" + device.DottedAddress;
			Device = device;
			_guid = device.UID;
		}

		bool _isTimerEnabled;
		public bool IsTimerEnabled
		{
			get { return _isTimerEnabled; }
			set
			{
				_isTimerEnabled = value;
				OnPropertyChanged(() => IsTimerEnabled);
			}
		}

		int _timeLeft;
		public int TimeLeft
		{
			get { return _timeLeft; }
			set
			{
				_timeLeft = value;
				OnPropertyChanged(() => TimeLeft);

				if (TimeLeft <= 0)
				{
					IsTimerEnabled = false;
					Close();
				}
			}
		}

		public void RestartTimer(int timeLeft)
		{
			if (TimeLeft <= 0)
				TimeLeft = timeLeft;
			StartTimer(TimeLeft);
		}

		public void StartTimer(int timeLeft)
		{
			TimeLeft = timeLeft;
			IsTimerEnabled = true;
			if (DispatcherTimer == null)
			{
				DispatcherTimer = new DispatcherTimer();
				DispatcherTimer.Interval = TimeSpan.FromSeconds(1);
				DispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
				DispatcherTimer.Start();
			}
		}

		void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			--TimeLeft;
		}

		public void Stop()
		{
			IsTimerEnabled = false;
			if (DispatcherTimer != null)
				DispatcherTimer.Stop();
			DispatcherTimer = null;
			Close();
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return "MPT_" + _guid.ToString(); }
		}
		#endregion
	}
}