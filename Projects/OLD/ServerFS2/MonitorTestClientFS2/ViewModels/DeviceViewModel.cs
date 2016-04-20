using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.TreeList;
using Infrastructure.Common.Windows.Windows;
using MonitorTestClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.Monitoring;
using ServerFS2.Processor;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			ResetCommand = new RelayCommand<DriverState>(OnReset, CanReset);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetGuardCommand = new RelayCommand(OnSetGuard, CanSetResetGuard);
			ResetGuardCommand = new RelayCommand(OnResetGuard, CanSetResetGuard);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ExecuteCommand = new RelayCommand(OnExecute, CanExecute);
			Device = device;
			PresentationZone = ConfigurationManager.DeviceConfiguration.GetPresentationZone(Device);
			InitializeCommands();
			device.DeviceState.StateChanged += new Action(OnStateChanged);
			device.DeviceState.ParametersChanged += new Action(OnParametersChanged);
		}

		public string PresentationZone { get; private set; }

		void OnStateChanged()
		{
			States = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeStates)
			{
				var stateViewModel = new StateViewModel()
				{
					DriverState = state.DriverState
				};
				States.Add(stateViewModel);
			}

			ParentStates = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeParentStates)
			{
				var stateViewModel = new StateViewModel()
				{
					DriverState = state.DriverState,
					DeviceName = state.ParentDevice.DottedPresentationAddressAndName
				};
				ParentStates.Add(stateViewModel);
			}

			ChildState = StateType.Norm;
			HasChildStates = false;
			foreach (var state in DeviceState.ThreadSafeChildStates)
			{
				if (state.StateType < ChildState)
				{
					ChildState = state.StateType;
					HasChildStates = true;
				}
			}

			OnPropertyChanged("StateType");
			OnPropertyChanged("ToolTip");
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("DeviceState.States");
			OnPropertyChanged("DeviceState.StringStates");
			OnPropertyChanged("DeviceState.ParentStringStates");
			OnPropertyChanged("States");
			OnPropertyChanged("ParentStates");
			OnPropertyChanged("ChildState");
			OnPropertyChanged("HasChildStates");
			OnPropertyChanged("Parameters");
		}

		public DeviceState DeviceState
		{
			get { return Device.DeviceState; }
		}

		public Driver Driver
		{
			get { return Device.Driver; }
		}

		public List<StateViewModel> States { get; private set; }
		public List<StateViewModel> ParentStates { get; private set; }
		public StateType ChildState { get; private set; }
		public bool HasChildStates { get; private set; }

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Device.PresentationAddressAndName);

				if (DeviceState.ParentStringStates != null)
				{
					foreach (var parentState in DeviceState.ParentStringStates)
					{
						stringBuilder.AppendLine(parentState);
					}
				}

				foreach (var state in DeviceState.ThreadSafeStates)
				{
					if (state.DriverState != null)
						stringBuilder.AppendLine(state.DriverState.Name);
				}

				foreach (var parameter in DeviceState.ThreadSafeParameters)
				{
					if (!parameter.IsIgnore && parameter.Visible && parameter.Value != "NAN")
					{
						stringBuilder.Append(parameter.Caption);
						stringBuilder.Append(" - ");
						stringBuilder.AppendLine(parameter.Value);
					}
				}

				var result = stringBuilder.ToString();
				if (result.EndsWith("\r\n"))
					result = result.Remove(result.Length - 2);
				return result;
			}
		}

		void OnParametersChanged()
		{
			OnPropertyChanged("Parameters");
		}

		public List<string> Parameters
		{
			get
			{
				var parameters = new List<string>();
				if (DeviceState != null)
					foreach (var parameter in DeviceState.ThreadSafeParameters)
					{
						if (!parameter.IsIgnore && parameter.Visible)
						{
							parameters.Add(parameter.Caption + ": " + parameter.Value);
						}
					}
				return parameters;
			}
		}

		public RelayCommand<DriverState> ResetCommand { get; private set; }
		void OnReset(DriverState driverState)
		{
			var panelResetItems = new List<PanelResetItem>();
			var panelResetItem = new PanelResetItem()
			{
				PanelUID = DeviceState.Device.UID
			};
			panelResetItem.Ids.Add(driverState.Code);
			panelResetItems.Add(panelResetItem);
			MainManager.ResetStates(panelResetItems, null);
		}
		bool CanReset(DriverState state)
		{
			return DeviceState.ThreadSafeStates.Any(x => (x.DriverState != null && x.DriverState == state && x.DriverState.IsManualReset));
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			MonitoringManager.AddTaskIgnore(new List<Device>() { Device });
		}
		bool CanSetIgnore()
		{
			return DeviceState.Device.Driver.CanDisable && !DeviceState.IsDisabled;
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			MonitoringManager.AddTaskResetIgnore(new List<Device>() { Device });
		}
		bool CanResetIgnore()
		{
			return DeviceState.Device.Driver.CanDisable && DeviceState.IsDisabled;
		}

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			MonitoringManager.AddTaskSetGuard(Device, "Пользователь", null);
		}

		public RelayCommand ResetGuardCommand { get; private set; }
		void OnResetGuard()
		{
			MonitoringManager.AddTaskResetGuard(Device, "Пользователь", null);
		}

		bool CanSetResetGuard()
		{
			return Device.Driver.DriverType == DriverType.Rubezh_2OP || Device.Driver.DriverType == DriverType.USB_Rubezh_2OP;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}

		void InitializeCommands()
		{
			DeviceCommands = new List<DeviceCommandViewModel>();

			var tableNo = MetadataHelper.GetDeviceTableNo(Device);
			var metadataDeviceCommands = MetadataHelper.Metadata.devicePropInfos.Where(x => x.tableType == tableNo);
			foreach (var metadataDeviceCommand in metadataDeviceCommands)
			{
				if (metadataDeviceCommand.commandDev != null)
				{
					var deviceCommandViewModel = new DeviceCommandViewModel()
					{
						Name = metadataDeviceCommand.name,
						Caption = metadataDeviceCommand.caption,
					};
					DeviceCommands.Add(deviceCommandViewModel);
				}
			}
			SelectedDeviceCommand = DeviceCommands.FirstOrDefault();
		}

		public List<DeviceCommandViewModel> DeviceCommands { get; private set; }

		DeviceCommandViewModel _selectedDeviceCommand;
		public DeviceCommandViewModel SelectedDeviceCommand
		{
			get { return _selectedDeviceCommand; }
			set
			{
				_selectedDeviceCommand = value;
				OnPropertyChanged("SelectedDeviceCommand");
			}
		}

		public RelayCommand ExecuteCommand { get; private set; }
		void OnExecute()
		{
			MainManager.ExecuteCommand(Device, SelectedDeviceCommand.Name, "Пользователь");
		}
		bool CanExecute()
		{
			return SelectedDeviceCommand != null && SelectedDeviceCommand != null;
		}
	}
}