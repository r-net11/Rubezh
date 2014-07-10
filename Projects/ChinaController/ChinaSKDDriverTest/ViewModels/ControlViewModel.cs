using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class ControlViewModel : BaseViewModel
	{
		public ControlViewModel()
		{
			OpenDoorCommand = new RelayCommand(OnOpenDoor);
			CloseDoorCommand = new RelayCommand(OnCloseDoor);
			GetDoorStatusCommand = new RelayCommand(OnGetDoorStatus);
		}

		public RelayCommand OpenDoorCommand { get; private set; }
		void OnOpenDoor()
		{
			var result = MainViewModel.Wrapper.OpenDoor(Index);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand CloseDoorCommand { get; private set; }
		void OnCloseDoor()
		{
			var result = MainViewModel.Wrapper.CloseDoor(Index);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand GetDoorStatusCommand { get; private set; }
		void OnGetDoorStatus()
		{
			var result = MainViewModel.Wrapper.GetDoorStatus(Index);
			switch (result)
			{
				case -1:
					MessageBox.Show("Error");
					break;

				case 0:
					MessageBox.Show("Unknown");
					break;

				case 1:
					MessageBox.Show("Opened");
					break;

				case 2:
					MessageBox.Show("Closed");
					break;
			}
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
	}
}