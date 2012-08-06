using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DirectionDeviceViewModel : BaseViewModel
	{
		public DirectionDeviceViewModel(DirectionDevice directionDevice = null)
		{
			ChooseDeviceCommand = new RelayCommand(OnChooseDevice);

			StateTypes = new List<XStateType>();
			StateTypes.Add(XStateType.TurnOn);
			StateTypes.Add(XStateType.CancelDelay);
			StateTypes.Add(XStateType.TurnOff);
			StateTypes.Add(XStateType.Stop);
			StateTypes.Add(XStateType.ForbidStart);
			StateTypes.Add(XStateType.TurnOnNow);
			StateTypes.Add(XStateType.TurnOffNow);
			SelectedStateType = StateTypes.FirstOrDefault();

			if (directionDevice != null)
			{
				SelectedStateType = directionDevice.StateType;
				if (directionDevice.DeviceUID != null)
					Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == directionDevice.DeviceUID);
			}
		}

		public List<XStateType> StateTypes { get; private set; }

		XStateType _selectedStateType;
		public XStateType SelectedStateType
		{
			get { return _selectedStateType; }
			set
			{
				_selectedStateType = value;
				OnPropertyChanged("SelectedStateType");
			}
		}

		XDevice _device;
		public XDevice Device
		{
			get { return _device; }
			set
			{
				_device = value;
				OnPropertyChanged("Device");
			}
		}

		public RelayCommand ChooseDeviceCommand { get; private set; }
		void OnChooseDevice()
		{
			var directionDeviceSelectationViewModel = new DirectionDeviceSelectationViewModel();

			if (DialogService.ShowModalWindow(directionDeviceSelectationViewModel))
				Device = directionDeviceSelectationViewModel.SelectedDevice.Device;
		}
	}
}