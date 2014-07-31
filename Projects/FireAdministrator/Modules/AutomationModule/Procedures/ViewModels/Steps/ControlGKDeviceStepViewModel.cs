using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
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
			Commands = new ObservableCollection<string>();
			SelectDeviceCommand = new RelayCommand(OnSelectDevice);
			UpdateContent();
		}

		public ObservableCollection<string> Commands { get; private set; }

		string _selectedCommand;
		public string SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGkDeviceArguments.Command = StringToXStateBit(value);
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				ControlGkDeviceArguments.DeviceUid = Guid.Empty;
				if (_selectedDevice != null)
				{
					ControlGkDeviceArguments.DeviceUid = _selectedDevice.Device.UID;
					InitializeCommands(_selectedDevice.Device);
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		void InitializeCommands(XDevice device)
		{
			if (IsBiStateControl(device))
			{
				Commands = new ObservableCollection<string> { "Снять отключение", "Отключение" };
				if (HasReset(device))
					Commands.Add("Сбросить");
			}
			if (IsTriStateControl(device))
			{
				Commands = new ObservableCollection<string> { "Автоматика", "Ручное", "Отключение" };
				foreach (var availableCommand in device.Driver.AvailableCommandBits)
				{
					if (device.DriverType == XDriverType.Valve)
					{
						switch (availableCommand)
						{
							case XStateBit.TurnOn_InManual:
								Commands.Add("Открыть");
								break;
							case XStateBit.TurnOnNow_InManual:
								Commands.Add("Открыть немедленно");
								break;
							case XStateBit.TurnOff_InManual:
								Commands.Add("Закрыть");
								break;
							case XStateBit.Stop_InManual:
								Commands.Add("Остановить");
								break;
						}
					}
					else
						Commands.Add(availableCommand.ToDescription());
				}
				if (device.DriverType == XDriverType.JockeyPump)
					Commands.Add("Запретить пуск");
			}
			OnPropertyChanged(() => Commands);
			if (String.IsNullOrEmpty(SelectedCommand))
				SelectedCommand = Commands.FirstOrDefault();
		}

		public bool IsBiStateControl(XDevice device)
		{
			return device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice;
		}

		public bool IsTriStateControl(XDevice device)
		{
			return device.Driver.IsControlDevice;
		}

		public bool HasReset(XDevice device)
		{
			return device.DriverType == XDriverType.AMP_1 || device.DriverType == XDriverType.RSR2_MAP4;
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
				SelectedCommand = XStateBitToString(ControlGkDeviceArguments.Command);
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public string Description
		{
			get { return ""; }
		}
		
		XStateBit StringToXStateBit (string stateString)
		{
			switch (stateString)
			{
				case "Автоматика":
					return XStateBit.SetRegime_Automatic;
				case "Снять отключение":
					return XStateBit.SetRegime_Automatic;
				case "Ручное":
					return XStateBit.SetRegime_Manual;
				case"Отключение":
					return XStateBit.Ignore;
				case "Включить":
					return XStateBit.TurnOn_InManual;
				case "Открыть":
					return XStateBit.TurnOn_InManual;
				case "Включить немедленно":
					return XStateBit.TurnOnNow_InManual;
				case "Открыть немедленно":
					return XStateBit.TurnOnNow_InManual;
				case "Выключить":
					return XStateBit.TurnOff_InManual;
				case "Закрыть":
					return XStateBit.TurnOff_InManual;
				case "Остановить":
					return XStateBit.Stop_InManual;
				case "Сбросить":
					return XStateBit.Reset;
				case "Запретить пуск":
					return XStateBit.ForbidStart_InManual;
				default:
					return new XStateBit();
			}
		}

		string XStateBitToString(XStateBit stateString)
		{
			switch (stateString)
			{
				case XStateBit.SetRegime_Automatic:
					return IsTriStateControl(SelectedDevice.Device)? "Автоматика": "Cнять отключение";
				case XStateBit.SetRegime_Manual:
					return "Ручное";
				case XStateBit.Ignore:
					return "Отключение";
				case XStateBit.TurnOn_InManual:
					return (SelectedDevice.Device.DriverType == XDriverType.Valve) ? "Открыть" : "Включить";
				case XStateBit.TurnOnNow_InManual:
					return (SelectedDevice.Device.DriverType == XDriverType.Valve) ? "Открыть немедленно" : "Включить немедленно";
				case XStateBit.TurnOff_InManual:
					return (SelectedDevice.Device.DriverType == XDriverType.Valve) ? "Закрыть" : "Выключить";
				case XStateBit.Stop_InManual:
					return "Остановить";
				case XStateBit.Reset:
					return "Сбросить";
				case XStateBit.ForbidStart_InManual:
					return "Запретить пуск";
				default:
					return "";
			}
		}
	}
}
