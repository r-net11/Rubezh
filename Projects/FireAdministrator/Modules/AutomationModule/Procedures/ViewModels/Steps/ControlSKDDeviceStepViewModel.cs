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
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDDeviceStepViewModel(ControlSKDDeviceArguments controlSKDDeviceArguments, Procedure procedure)
		{
			ControlSKDDeviceArguments = controlSKDDeviceArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<SKDDeviceCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDDeviceArguments.Variable1);
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
				Variable1.UidValue = Guid.Empty;
				if (_selectedDevice != null)
				{
					Variable1.UidValue = _selectedDevice.SKDDevice.UID;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.SKDDevice, false));
			if (Variable1.UidValue != Guid.Empty)
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == Variable1.UidValue);
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

