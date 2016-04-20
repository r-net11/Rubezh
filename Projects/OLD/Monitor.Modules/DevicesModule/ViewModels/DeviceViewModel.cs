using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.TreeList;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Events;
using Infrastructure.Models;

namespace DevicesModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }

		public DeviceViewModel(Device device)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			SetGuardCommand = new RelayCommand(OnSetGuard);
			UnsetGuardCommand = new RelayCommand(OnUnsetGuard);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ResetCommand = new RelayCommand<string>(OnReset, CanReset);

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

			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
			PresentationAddress = Device.PresentationAddress;
		}

		public string PresentationZone { get; private set; }
		public string PresentationAddress { get; private set; }

		void OnStateChanged()
		{
			States = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeStates)
			{
				var stateViewModel = new StateViewModel(state.DriverState, DeviceState.Device);
				States.Add(stateViewModel);
			}

			ParentStates = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeParentStates)
			{
				var stateViewModel = new StateViewModel(state.DriverState)
				{
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

			OnPropertyChanged(() => ToolTip);
			OnPropertyChanged(() => DeviceState);
			OnPropertyChanged(() => States);
			OnPropertyChanged(() => ParentStates);
			OnPropertyChanged(() => ChildState);
			OnPropertyChanged(() => HasChildStates);

			ShowTimerDetails();
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
			FailureType = "";
			AlarmReason = "";
			Smokiness = "";
			Dustiness = "";
			Temperature = "";

			if (DeviceState != null)
			{
				foreach (var parameter in DeviceState.ThreadSafeParameters)
				{
					string parameterValue = parameter.Value;
					if (parameter.IsIgnore)
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
			OnPropertyChanged(() => Parameters);
		}

		public List<string> Parameters
		{
			get
			{
				if (DeviceState != null)
				{
					return DeviceState.StateParameters;
				}
				return new List<string>();
			}
		}

		string _failureType;
		public string FailureType
		{
			get { return _failureType; }
			set
			{
				_failureType = value;
				OnPropertyChanged(() => FailureType);
			}
		}

		string _alarmReason;
		public string AlarmReason
		{
			get { return _alarmReason; }
			set
			{
				_alarmReason = value;
				OnPropertyChanged(() => AlarmReason);
			}
		}

		string _smokiness;
		public string Smokiness
		{
			get { return _smokiness; }
			set
			{
				_smokiness = value;
				OnPropertyChanged(() => Smokiness);
			}
		}

		string _dustiness;
		public string Dustiness
		{
			get { return _dustiness; }
			set
			{
				_dustiness = value;
				OnPropertyChanged(() => Dustiness);
			}
		}

		string _temperature;
		public string Temperature
		{
			get { return _temperature; }
			set
			{
				_temperature = value;
				OnPropertyChanged(() => Temperature);
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

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
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

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.ChangeDisabled(DeviceState);
			}
		}
		public bool CanDisable()
		{
			return FiresecManager.CanDisable(DeviceState);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(new ElementDeviceReference() { DeviceUID = Device.UID });
		}
		bool CanShowProperties()
		{
			return !Device.Driver.IsChildAddressReservedRange;
		}

		public RelayCommand<string> ResetCommand { get; private set; }
		void OnReset(string stateName)
		{
			var resetItems = new List<ResetItem>();
			var resetItem = new ResetItem()
			{
				DeviceState = DeviceState
			};
			var deviceDriverState = DeviceState.ThreadSafeStates.FirstOrDefault(x => x.DriverState.Name == stateName);
			resetItem.States.Add(deviceDriverState);
			resetItems.Add(resetItem);
			FiresecManager.ResetStates(resetItems);

			OnPropertyChanged(() => DeviceState);
		}
		bool CanReset(string stateName)
		{
			return DeviceState.ThreadSafeStates.Any(x => (x.DriverState.Name == stateName && x.DriverState.IsManualReset));
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

		public string GuardDeviceType
		{
			get
			{
				if (Device.Driver.DriverType == DriverType.AM1_O)
				{
					var property = Device.Properties.FirstOrDefault(x => x.Name == "GuardType");
					if (property == null)
						return null;
					if (property.Value == null)
						return null;
					var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == "GuardType");
					if (driverProperty == null)
						return null;
					var propertyParameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
					if (propertyParameter == null)
						return null;
					return "(" + propertyParameter.Name + ")";
				}
				return null;
			}
		}

		void ShowTimerDetails()
		{
			if (Device.Driver.DriverType == DriverType.MPT && !FiresecManager.FiresecConfiguration.IsChildMPT(Device))
			{
				var deviceDriverState = DeviceState.ThreadSafeStates.FirstOrDefault(x => x.DriverState.Code == "MPT_On");
				if (deviceDriverState != null)
				{
					if (DateTime.Now > deviceDriverState.Time)
					{
						var timeSpan = DateTime.Now - deviceDriverState.Time;

						var timeoutProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == "AU_Delay");
						if (timeoutProperty != null)
						{
							int timeout = 0;
							try
							{
								timeout = int.Parse(timeoutProperty.Value);
							}
							catch (Exception e)
							{
								Logger.Error(e, "DeviceViewModel.ShowTimerDetails");
								return;
							}

							int secondsLeft = timeout - (int)timeSpan.Value.TotalSeconds;
							if (secondsLeft > 0)
							{
								if (Device.Zone != null && Device.Zone.EnableExitTime)
								{
									if (MPTTimerViewModel == null)
										MPTTimerViewModel = new MPTTimerViewModel(Device);
									DialogService.ShowWindow(MPTTimerViewModel);
									if (Device.Zone.ExitRestoreType == ExitRestoreType.SetTimer)
										MPTTimerViewModel.StartTimer(secondsLeft);
									else
										MPTTimerViewModel.RestartTimer(secondsLeft);
								}
							}
						}
					}
				}
				else
				{
					if (MPTTimerViewModel != null)
						MPTTimerViewModel.Stop();
				}
			}
		}

		MPTTimerViewModel MPTTimerViewModel { get; set; }
		public bool IsBold { get; set; }
	}
}