using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }

		public DeviceViewModel(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
		{
			ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowOnPlan);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			SetGuardCommand = new RelayCommand(OnSetGuard);
			UnsetGuardCommand = new RelayCommand(OnUnsetGuard);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ResetCommand = new RelayCommand<string>(OnReset, CanReset);

			Source = sourceDevices;
			Device = device;
			DeviceState = Device.DeviceState;
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new Action(OnStateChanged);
				DeviceState.ParametersChanged += new Action(OnParametersChanged);
				OnStateChanged();
				OnParametersChanged();
			}
			else
			{
				string deviceName = Device.AddressFullPath + " - " + device.Driver.Name + "." + device.PresentationAddress;
				string errorText = "Ошибка при сопоставлении устройства с его состоянием:\n" + deviceName;
				Logger.Warn(errorText);
			}
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		void OnStateChanged()
		{
			if (DeviceState.States == null)
			{
				Logger.Error("DeviceViewModel.OnStateChanged: DeviceState.States = null");
				return;
			}

			States = new List<StateViewModel>();
			foreach (var state in DeviceState.States)
			{
				var stateViewModel = new StateViewModel()
				{
					DriverState = state.DriverState
				};
				States.Add(stateViewModel);
			}

			ParentStates = new List<StateViewModel>();
			foreach (var state in DeviceState.ParentStates)
			{
				var stateViewModel = new StateViewModel()
				{
					DriverState = state.DriverState,
					DeviceName = state.ParentDevice.Driver.ShortName + " - " + state.ParentDevice.DottedAddress
				};
				ParentStates.Add(stateViewModel);
			}

			ChildState = StateType.Norm;
			HasChildStates = false;
			foreach (var state in DeviceState.ChildStates)
			{
				if (state.DriverState.StateType < ChildState)
				{
					ChildState = state.DriverState.StateType;
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
				stringBuilder.Append(Device.PresentationAddress);
				stringBuilder.Append(" - ");
				stringBuilder.Append(Device.Driver.ShortName);
				stringBuilder.Append("\n");

				if (DeviceState.ParentStringStates != null)
				{
					foreach (var parentState in DeviceState.ParentStringStates)
					{
						stringBuilder.AppendLine(parentState);
					}
				}

				foreach (var state in DeviceState.States)
				{
					stringBuilder.AppendLine(state.DriverState.Name);
				}

				if (DeviceState.Parameters != null)
				{
					var nullString = "<NULL>";
					foreach (var parameter in DeviceState.Parameters)
					{
						if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == nullString)
							continue;
						if ((parameter.Name == "Config$SerialNum") || (parameter.Name == "Config$SoftVersion"))
							continue;

						stringBuilder.Append(parameter.Caption);
						stringBuilder.Append(" - ");
						stringBuilder.AppendLine(parameter.Value);
					}
				}

				return stringBuilder.ToString();
			}
		}

		void OnParametersChanged()
		{
			if (DeviceState != null && DeviceState.Parameters.IsNotNullOrEmpty())
			{
				foreach (var parameter in DeviceState.Parameters)
				{
					string parameterValue = parameter.Value;
					if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
						parameterValue = " - ";

					switch (parameter.Name)
					{
						case "FailureType":
							FailureType = parameterValue;
							break;

						case "AlarmReason":
							AlarmReason = parameterValue;
							break;

						case "Smokiness":
							Smokiness = parameterValue;
							break;

						case "Dustiness":
							Dustiness = parameterValue;
							break;

						case "Temperature":
							Temperature = parameterValue;
							break;

						default:
							break;
					}
				}
			}
			OnPropertyChanged("Parameters");
		}

		public List<string> Parameters
		{
			get
			{
				var parameters = new List<string>();
				if (DeviceState != null && DeviceState.Parameters.IsNotNullOrEmpty())
					foreach (var parameter in DeviceState.Parameters)
					{
						if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == "<NULL>")
							continue;
						if ((parameter.Name == "Config$SerialNum") || (parameter.Name == "Config$SoftVersion"))
							continue;
						parameters.Add(parameter.Caption + " - " + parameter.Value);
					}
				return parameters;
			}
		}

		string _failureType;
		public string FailureType
		{
			get { return _failureType; }
			set
			{
				_failureType = value;
				OnPropertyChanged("FailureType");
			}
		}

		string _alarmReason;
		public string AlarmReason
		{
			get { return _alarmReason; }
			set
			{
				_alarmReason = value;
				OnPropertyChanged("AlarmReason");
			}
		}

		string _smokiness;
		public string Smokiness
		{
			get { return _smokiness; }
			set
			{
				_smokiness = value;
				OnPropertyChanged("Smokiness");
			}
		}

		string _dustiness;
		public string Dustiness
		{
			get { return _dustiness; }
			set
			{
				_dustiness = value;
				OnPropertyChanged("Dustiness");
			}
		}

		string _temperature;
		public string Temperature
		{
			get { return _temperature; }
			set
			{
				_temperature = value;
				OnPropertyChanged("Temperature");
			}
		}

		public bool CanShowOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementDevices.Any(x => x.DeviceUID == Device.UID))
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand ShowPlanCommand { get; private set; }
		void OnShowPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Device.UID);
		}

		public bool CanShowZone()
		{
            return ((Device.Driver.IsZoneDevice) && (Device.ZoneUID != Guid.Empty));
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneUID);
		}

		public bool CanDisable()
		{
			return FiresecManager.CanDisable(DeviceState);
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.ChangeDisabled(DeviceState);
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(Device.UID);
		}

		public RelayCommand<string> ResetCommand { get; private set; }
		void OnReset(string stateName)
		{
			var resetItems = new List<ResetItem>();
			var resetItem = new ResetItem()
			{
				DeviceState = DeviceState
			};
			var deviceDriverState = DeviceState.States.FirstOrDefault(x => x.DriverState.Name == stateName);
			resetItem.States.Add(deviceDriverState);
			resetItems.Add(resetItem);
            FiresecManager.FiresecDriver.ResetStates(resetItems);

			OnPropertyChanged("DeviceState");
		}
		bool CanReset(string stateName)
		{
			return DeviceState.States.Any(x => (x.DriverState.Name == stateName && x.DriverState.IsManualReset));
		}

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.SetDeviceGuard(Device);
		}

		public RelayCommand UnsetGuardCommand { get; private set; }
		void OnUnsetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.UnSetDeviceGuard(Device);
		}

		public bool IsGuardDevice
		{
			get { return Device.Driver.DriverType == DriverType.Rubezh_2OP || Device.Driver.DriverType == DriverType.USB_Rubezh_2OP; }
		}
	}
}