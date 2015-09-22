using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class ControlViewModel : BaseViewModel
	{
		private DispatcherTimer _timer;

		public ControlViewModel()
		{
			InitAvailableDoorStatuses();			
			OpenDoorCommand = new RelayCommand(OnOpenDoor);
			CloseDoorCommand = new RelayCommand(OnCloseDoor);
			GetDoorStatusCommand = new RelayCommand(OnGetDoorStatus);
			PromptWarningCommand = new RelayCommand(OnPromptWarning);
			
			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromMilliseconds(5000);
			_timer.Tick += (sender, args) =>
			{
				OnGetDoorStatus();
				_timer.Stop();
			};
		}

		public RelayCommand OpenDoorCommand { get; private set; }
		void OnOpenDoor()
		{
			if (!MainViewModel.Wrapper.OpenDoor(Index))
			{
				MessageBox.Show("Операция открытия двери завершилась с ошибкой");
				return;
			}
			DoorStatus = (DoorStatus)MainViewModel.Wrapper.GetDoorStatus(Index);
			_timer.Stop();
			_timer.Start();
		}

		public RelayCommand CloseDoorCommand { get; private set; }
		void OnCloseDoor()
		{
			if (!MainViewModel.Wrapper.CloseDoor(Index))
			{
				MessageBox.Show("Операция закрытия двери завершилась с ошибкой");
				return;
			}
			DoorStatus = (DoorStatus)MainViewModel.Wrapper.GetDoorStatus(Index);
			_timer.Stop();
			_timer.Start();
		}

		public RelayCommand GetDoorStatusCommand { get; private set; }
		void OnGetDoorStatus()
		{
			DoorStatus = (DoorStatus)MainViewModel.Wrapper.GetDoorStatus(Index);
		}

		public RelayCommand PromptWarningCommand { get; private set; }
		private void OnPromptWarning()
		{
			var opStatus = MainViewModel.Wrapper.PromptWarning(Index) ? "успешно" : "с ошибкой";
			MessageBox.Show(String.Format("Операция сброса состояния \"ВЗЛОМ\" завершилась {0}", opStatus));
		}

		int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				OnPropertyChanged(() => Index);
			}
		}

		private DoorStatus _doorStatus;
		public DoorStatus DoorStatus
		{
			get { return _doorStatus; }
			set
			{
				if (_doorStatus == value)
					return;
				_doorStatus = value;
				OnPropertyChanged(() => DoorStatus);
			}
		}

		public ObservableCollection<DoorStatus> AvailableDoorStatuses { get; private set; }

		private void InitAvailableDoorStatuses()
		{
			AvailableDoorStatuses = new ObservableCollection<DoorStatus>
			{
				DoorStatus.Error,
				DoorStatus.Unknown,
				DoorStatus.Opened,
				DoorStatus.Closed,
				DoorStatus.Break
			};
		}

	}

	public enum DoorStatus
	{
		Error = -1,
		Unknown = 0,
		Opened = 1,
		Closed = 2,
		Break = 3
	}
}