using System;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlGKDeviceStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlGKDeviceArguments ControlGkDeviceArguments { get; set; }
		public ControlGKDeviceStepViewModel(ControlGKDeviceArguments controlGkDeviceArguments)
		{
			ControlGkDeviceArguments = controlGkDeviceArguments;
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);
			UpdateContent();
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				ControlGkDeviceArguments.DeviceUid = _selectedDevice != null ? _selectedDevice.Device.UID : Guid.Empty;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		private void OnSelectDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectionViewModel(SelectedDevice != null ? SelectedDevice.Device : null);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				SelectedDevice = deviceSelectationViewModel.SelectedDevice;
			}
		}

		public void UpdateContent()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if (ControlGkDeviceArguments.DeviceUid != Guid.Empty)
			{
				var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == ControlGkDeviceArguments.DeviceUid);
				SelectedDevice = device != null ? new DeviceViewModel(device) : null;
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
