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
	public class ControlGKFireZoneStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		public ControlGKFireZoneStepViewModel(ControlGKFireZoneArguments controlGKFireZoneArguments)
		{
			ControlGKFireZoneArguments = controlGKFireZoneArguments;
			Commands = new ObservableCollection<string>();
			SelectZoneCommand = new RelayCommand(OnSelectZone);
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
				//ControlGKFireZoneArguments.Command = StringToXStateBit(value);
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				ControlGKFireZoneArguments.ZoneUid = Guid.Empty;
				if (_selectedZone != null)
				{
					ControlGKFireZoneArguments.ZoneUid = _selectedZone.Zone.UID;
					//InitializeCommands(_selectedZone.Zone);
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedZone);
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

		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new ZoneSelectionViewModel(SelectedZone != null ? SelectedZone.Zone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if (ControlGKFireZoneArguments.ZoneUid != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == ControlGKFireZoneArguments.ZoneUid);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				//SelectedCommand = XStateBitToString(ControlGKFireZoneArguments.Command);
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
	}
}
