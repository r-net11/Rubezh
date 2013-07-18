using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using ServerFS2.Monitoring;
using ServerFS2.Processor;
using System.Diagnostics;
using System;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using MonitorTestClientFS2.ViewModels;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			ResetCommand = new RelayCommand<DriverState>(OnReset, CanReset);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore);
			SetGuardCommand = new RelayCommand(OnSetGuard);
			ResetGuardCommand = new RelayCommand(OnResetGuard);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			TurnOnRMTestCommand = new RelayCommand(OnTurnOnRMTest);
			TurnOffRMTestCommand = new RelayCommand(OnTurnOffRMTest);
			ExecuteCommand = new RelayCommand(OnExecute);
			DeviceCommands = new List<DeviceCommandViewModel>();
			Device = device;
			device.DeviceState.StateChanged += new Action(OnStateChanged);
			device.DeviceState.ParametersChanged += new Action(OnParametersChanged);
		}

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

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			MonitoringManager.AddTaskResetIgnore(new List<Device>() { Device });
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

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}

		public RelayCommand TurnOnRMTestCommand { get; private set; }
		void OnTurnOnRMTest()
		{
			MainManager.ExecuteCommand(Device, "RunWODelay", "Пользователь");
		}

		public RelayCommand TurnOffRMTestCommand { get; private set; }
		void OnTurnOffRMTest()
		{
			MainManager.ExecuteCommand(Device, "Stop", "Пользователь");
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
			if (SelectedDeviceCommand != null)
			{
				ServerHelper.ExecuteCommand(Device, SelectedDeviceCommand.Name);
			}
		}
	}
}