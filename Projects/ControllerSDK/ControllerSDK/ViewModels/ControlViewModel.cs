using System.Windows;
using ChinaSKDDriverNativeApi;
using ControllerSDK.Views;
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
			var result = NativeWrapper.WRAP_DevCtrl_OpenDoor(MainWindow.LoginID);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand CloseDoorCommand { get; private set; }
		void OnCloseDoor()
		{
			var result = NativeWrapper.WRAP_DevCtrl_CloseDoor(MainWindow.LoginID);
			MessageBox.Show(result.ToString());
		}

		public RelayCommand GetDoorStatusCommand { get; private set; }
		void OnGetDoorStatus()
		{
			var result = NativeWrapper.WRAP_DevState_DoorStatus(MainWindow.LoginID);
			switch(result)
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
	}
}