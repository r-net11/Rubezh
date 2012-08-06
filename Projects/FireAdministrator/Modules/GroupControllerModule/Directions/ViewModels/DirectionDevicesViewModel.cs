using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DirectionDevicesViewModel
	{
		XDirection Direction;

		public DirectionDevicesViewModel(XDirection direction)
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<DirectionDeviceViewModel>(OnRemove);

			Direction = direction;

			DirectionDevices = new ObservableCollection<DirectionDeviceViewModel>();
			if (direction.DirectionDevices == null)
				direction.DirectionDevices = new List<DirectionDevice>();
			foreach (var directionDevice in direction.DirectionDevices)
			{
				var directionDeviceViewModel = new DirectionDeviceViewModel(directionDevice);
				DirectionDevices.Add(directionDeviceViewModel);
			}
		}

		public ObservableCollection<DirectionDeviceViewModel> DirectionDevices { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var directionDeviceViewModel = new DirectionDeviceViewModel();
			DirectionDevices.Add(directionDeviceViewModel);
		}

		public RelayCommand<DirectionDeviceViewModel> RemoveCommand { get; private set; }
		void OnRemove(DirectionDeviceViewModel directionDeviceViewModel)
		{
			DirectionDevices.Remove(directionDeviceViewModel);
		}

		public void Save()
		{
			Direction.DirectionDevices = new List<DirectionDevice>();
			foreach (var directionDeviceViewModel in DirectionDevices)
			{
				if (directionDeviceViewModel.Device != null)
				{
					var directionDevice = new DirectionDevice()
					{
						DeviceUID = directionDeviceViewModel.Device.UID,
						StateType = directionDeviceViewModel.SelectedStateType
					};
					Direction.DirectionDevices.Add(directionDevice);
				}
			}
		}
	}
}