using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Controls.MessageBox;
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
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ResetCommand = new RelayCommand<string>(OnReset, CanReset);

			Source = sourceDevices;
			Device = device;

			DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new System.Action(OnStateChanged);
				DeviceState.ParametersChanged += new System.Action(OnParametersChanged);
				OnStateChanged();
				OnParametersChanged();
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка при сопоставлении устройства с его состоянием");
			}
		}

		public string PresentationZone
		{
			get { return Device.GetPersentationZone(); }
		}

		void OnStateChanged()
		{
			OnPropertyChanged("StateType");
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("DeviceState.States");
			OnPropertyChanged("DeviceState.StringStates");
			OnPropertyChanged("DeviceState.ParentStringStates");
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
		}

		public List<string> Parameters
		{
			get
			{
				var parameters = new List<string>();
				if (DeviceState != null && DeviceState.Parameters.IsNotNullOrEmpty())
					foreach (var parameter in DeviceState.Parameters)
					{
						if (parameter.Visible)
						{
							if (string.IsNullOrEmpty(parameter.Value))
								continue;
							if (parameter.Value == "<NULL>")
								continue;
							parameters.Add(parameter.Caption + " - " + parameter.Value);
						}
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
			return ((Device.Driver.IsZoneDevice) && (Device.ZoneNo.HasValue));
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneNo);
		}

		public bool CanDisable()
		{
			return DeviceState.CanDisable();
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				DeviceState.ChangeDisabled();
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(Device.UID);
		}

		bool CanReset(string stateName)
		{
			return DeviceState.States.Any(x => (x.DriverState.Name == stateName && x.DriverState.IsManualReset));
		}

		public RelayCommand<string> ResetCommand { get; private set; }
		void OnReset(string stateName)
		{
			var resetItems = new List<ResetItem>();
			var resetItem = new ResetItem()
			{
				DeviceUID = Device.UID
			};
			resetItem.StateNames.Add(stateName);
			resetItems.Add(resetItem);
			FiresecManager.FiresecService.ResetStates(resetItems);

			OnPropertyChanged("DeviceState");
		}
	}
}