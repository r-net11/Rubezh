using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ValveControlViewModel : BaseViewModel
	{
		Device Device;
		bool IsBuisy = false;

		public ValveControlViewModel(Device device)
		{
			CloseCommand = new RelayCommand(OnClose);
			StopCommand = new RelayCommand(OnStop);
			OpenCommand = new RelayCommand(OnOpen);
			AutomaticOnCommand = new RelayCommand(OnAutomaticOn);
			AutomaticOffCommand = new RelayCommand(OnAutomaticOff);
			StartCommand = new RelayCommand(OnStart);
			CancelStartCommand = new RelayCommand(OnCancelStart);
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
			StopTimerCommand = new RelayCommand(OnStopTimer);

			Device = device;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			SelectedCommand = "BoltClose";
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			SelectedCommand = "BoltStop";
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			SelectedCommand = "BoltOpen";
		}

		public RelayCommand AutomaticOnCommand { get; private set; }
		void OnAutomaticOn()
		{
			SelectedCommand = "BoltAutoOn";
		}

		public RelayCommand AutomaticOffCommand { get; private set; }
		void OnAutomaticOff()
		{
			SelectedCommand = "BoltAutoOff";
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			if (HasActionProprty)
			{
				SelectedCommand = "BoltOpen";
			}
			else
			{
				SelectedCommand = "BoltClose";
			}
		}

		public RelayCommand CancelStartCommand { get; private set; }
		void OnCancelStart()
		{
			if (HasActionProprty)
			{
				SelectedCommand = "BoltClose";
			}
			else
			{
				SelectedCommand = "BoltOpen";
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

		string _selectedCommand;
		public string SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				OnPropertyChanged("SelectedCommand");
			}
		}

		bool CanConfirm()
		{
			return !string.IsNullOrEmpty(SelectedCommand) && IsBuisy == false;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			IsBuisy = true;
			if (ServiceFactory.SecurityService.Validate())
			{
				var thread = new Thread(DoConfirm);
				thread.Start();
			}
		}

		void DoConfirm()
		{
			var result = FiresecManager.FiresecService.ExecuteCommand(Device.UID, SelectedCommand);
			Dispatcher.BeginInvoke(new Action(() => { IsBuisy = false; OnPropertyChanged("ConfirmCommand"); }));
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