using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		public ControlSKDDeviceStepViewModel(ControlSKDDeviceArguments controlSKDDeviceArguments)
		{
			ControlSKDDeviceArguments = controlSKDDeviceArguments;
			Commands = new ObservableCollection<SKDDeviceCommandType>
			{
				SKDDeviceCommandType.Open, SKDDeviceCommandType.Close, 
				SKDDeviceCommandType.OpenForever, SKDDeviceCommandType.CloseForever
			};
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);
			UpdateContent();
		}

		public ObservableCollection<SKDDeviceCommandType> Commands { get; private set; }

		SKDDeviceCommandType _selectedCommand;
		public SKDDeviceCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDDeviceArguments.Command = value;
				OnPropertyChanged(()=>SelectedCommand);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				ControlSKDDeviceArguments.DeviceUid = Guid.Empty;
				if (_selectedDevice != null)
				{
					ControlSKDDeviceArguments.DeviceUid = _selectedDevice.SKDDevice.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		private void OnSelectDevice()
		{
			var deviceSelectationViewModel = new SKDDeviceSelectionViewModel(SelectedDevice != null ? SelectedDevice.SKDDevice : null);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				SelectedDevice = deviceSelectationViewModel.SelectedDevice;
			}
		}

		public void UpdateContent()
		{
			if (ControlSKDDeviceArguments.DeviceUid != Guid.Empty)
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == ControlSKDDeviceArguments.DeviceUid);
				SelectedDevice = device != null ? new DeviceViewModel(device) : null;
				if (SelectedDevice != null)
					SelectedCommand = ControlSKDDeviceArguments.Command;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}

