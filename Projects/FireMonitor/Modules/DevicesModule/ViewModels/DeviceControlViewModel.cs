using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceControlViewModel : BaseViewModel
	{
		Device Device;
		bool IsBuisy = false;
		DispatcherTimer DispatcherTimer;

		public DeviceControlViewModel(Device device)
		{
			Device = device;
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);

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
				OnPropertyChanged(() => SelectedBlock);
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
					DoConfirm();
					var thread = new Thread(() =>
					{
						Thread.Sleep(TimeSpan.FromSeconds(5));
						ApplicationService.BeginInvoke(() =>
						{
							IsBuisy = false;
							OnPropertyChanged(() => ConfirmCommand);
						});
					});
					thread.Name = "DeviceControlViewModel Confirm";
					thread.Start();
				}
				catch (Exception e)
				{
					IsBuisy = false;
					Logger.Error(e, "DeviceControlViewModel.OnConfirm");
				}
			}
			else
			{
				IsBuisy = false;
			}
		}

		void DoConfirm()
		{
			try
			{
				FiresecManager.ExecuteCommand(Device, SelectedBlock.SelectedCommand.Name);
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceControlViewModel.DoConfirm.ExecuteCommand");
			}
			try
			{
				ApplicationService.BeginInvoke(() =>
				{
					IsBuisy = false;
					OnPropertyChanged(() => ConfirmCommand);
				});
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceControlViewModel.DoConfirm.BeginInvoke");
			}
		}

		#region Timer
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
					IsTimerEnabled = false;
			}
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

		public void StopTimer()
		{
			TimeLeft = 0;
			if (DispatcherTimer != null)
				DispatcherTimer.Stop();
			DispatcherTimer = null;
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
				OnPropertyChanged(() => SelectedCommand);
			}
		}
	}
}