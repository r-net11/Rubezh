using System;
using System.Linq;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;
using Infrastructure;
using System.Threading;

namespace DevicesModule.ViewModels
{
	public class ValveControlViewModel : BaseViewModel
	{
		Device Device;
		string CommandName;

		public ValveControlViewModel(Device device)
		{
			CloseCommand = new RelayCommand(OnClose);
			StopCommand = new RelayCommand(OnStop);
			OpenCommand = new RelayCommand(OnOpen);
			AutomaticOnCommand = new RelayCommand(OnAutomaticOn);
			AutomaticOffCommand = new RelayCommand(OnAutomaticOff);
			StartCommand = new RelayCommand(OnStart);
			CancelStartCommand = new RelayCommand(OnCancelStart);
			ConfirmCommand = new RelayCommand(OnConfirm);
			StopTimerCommand = new RelayCommand(OnStopTimer);

			Device = device;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			CommandName = "BoltClose";
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			CommandName = "BoltStop";
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			CommandName = "BoltOpen";
		}

		public RelayCommand AutomaticOnCommand { get; private set; }
		void OnAutomaticOn()
		{
			CommandName = "BoltAutoOn";
		}

		public RelayCommand AutomaticOffCommand { get; private set; }
		void OnAutomaticOff()
		{
			CommandName = "BoltAutoOff";
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			if (HasActionProprty)
			{
				CommandName = "BoltOpen";
			}
			else
			{
				CommandName = "BoltClose";
			}
		}

		public RelayCommand CancelStartCommand { get; private set; }
		void OnCancelStart()
		{
			if (HasActionProprty)
			{
				CommandName = "BoltClose";
			}
			else
			{
				CommandName = "BoltOpen";
			}
		}

		bool HasActionProprty
		{
			get
			{
				var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
				if (actionProperty != null)
				{
					return actionProperty.Value == "1";
				}
				return false;
			}
		}

		bool CanConfirmCommand()
		{
			return !string.IsNullOrEmpty(CommandName);
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			var thread = new Thread(DoConfirm);
			thread.Start();
		}

		bool IsBuisy = false;

		void DoConfirm()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				IsBuisy = true;
				var result = FiresecManager.FiresecService.ExecuteCommand(Device.UID, CommandName);
				IsBuisy = false;
				OnPropertyChanged("ConfirmCommand");
			}
		}

		bool _isTimerEnabled;
		public bool IsTimerEnabled
		{
			get { return _isTimerEnabled; }
			set
			{
				_isTimerEnabled = value;
				OnPropertyChanged("IsTimerEnabled");
			}
		}

		int _timeLeft;
		public int TimeLeft
		{
			get { return _timeLeft; }
			set
			{
				_timeLeft = value;
				OnPropertyChanged("TimeLeft");

				if (TimeLeft <= 0)
					IsTimerEnabled = false;
			}
		}

		public void StartTimer(int timeLeft)
		{
			TimeLeft = timeLeft;
			IsTimerEnabled = true;
			var dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
			dispatcherTimer.Start();
		}

		void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			--TimeLeft;
		}

		public RelayCommand StopTimerCommand { get; private set; }
		void OnStopTimer()
		{
			TimeLeft = 0;
		}
	}
}