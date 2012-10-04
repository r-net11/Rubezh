using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace DevicesModule.ViewModels
{
	public class DeviceControlViewModel : BaseViewModel
	{
		Device Device;
		bool IsBuisy = false;

		public DeviceControlViewModel(Device device)
		{
			Device = device;
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
            StopTimerCommand = new RelayCommand(OnStopTimer);

			Blocks = new List<BlockViewModel>();

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsControl)
				{
					var blockViewModel = Blocks.FirstOrDefault(x => x.Name == property.BlockName);
					if (blockViewModel == null)
					{
						blockViewModel = new BlockViewModel()
						{
							Name = property.BlockName
						};
						Blocks.Add(blockViewModel);
					}
					blockViewModel.Commands.Add(property);
				}
			}
		}

		public List<BlockViewModel> Blocks { get; private set; }

		BlockViewModel _selectedBlock;
		public BlockViewModel SelectedBlock
		{
			get { return _selectedBlock; }
			set
			{
				_selectedBlock = value;
				OnPropertyChanged("SelectedBlock");
			}
		}

		bool CanConfirm()
		{
			return SelectedBlock != null && SelectedBlock.SelectedCommand != null && IsBuisy == false;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			IsBuisy = true;
			if (ServiceFactory.SecurityService.Validate())
			{
				try
				{
					var thread = new Thread(DoConfirm);
					thread.Start();
				}
				catch (Exception e)
				{
					Logger.Error(e, "DeviceControlViewModel.OnConfirm");
				}
			}
		}

		void DoConfirm()
		{
			try
			{
				var result = FiresecManager.FiresecDriver.ExecuteCommand(Device.UID, GetCommandName());
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceControlViewModel.DoConfirm.ExecuteCommand");
			}

			try
			{
				Dispatcher.BeginInvoke(new Action(() => { IsBuisy = false; OnPropertyChanged("ConfirmCommand"); }));
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceControlViewModel.DoConfirm.BeginInvoke");
			}
		}

		#region Valve
		string GetCommandName()
		{
			var commandName = SelectedBlock.SelectedCommand.Name;
			if (Device.Driver.DriverType == DriverType.Valve)
			{
				if (!HasActionProprty())
				{
					if (commandName == "BoltOpen")
						return "BoltClose";
					if (commandName == "BoltClose")
						return "BoltOpen";
				}
			}
			return commandName;
		}

		bool HasActionProprty()
		{
			var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
			if (actionProperty != null)
			{
				return actionProperty.Value == "1";
			}
			return false;
		}
		#endregion

        #region Timer
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
        #endregion
    }

	public class BlockViewModel : BaseViewModel
	{
		public BlockViewModel()
		{
			Commands = new List<DriverProperty>();
		}

		public string Name { get; set; }
		public List<DriverProperty> Commands { get; set; }

		DriverProperty _selectedCommand;
		public DriverProperty SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				OnPropertyChanged("SelectedCommand");
			}
		}
	}
}