using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Device Device { get; private set; }
		public Guid AlternativeLibraryDeviceUID { get; private set; }
		public DeviceState DeviceState { get; private set; }
		public DeviceControlViewModel DeviceControlViewModel { get; private set; }

		public DeviceDetailsViewModel(Device device, Guid alternativeLibraryDeviceUID)
		{
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			Device = device;
			AlternativeLibraryDeviceUID = alternativeLibraryDeviceUID;
			DeviceState = Device.DeviceState;
			DeviceControlViewModel = new DeviceControlViewModel(Device);
			DeviceState.StateChanged += new Action(OnStateChanged);
			DeviceState.ParametersChanged += new Action(OnParametersChanged);
			OnStateChanged();

			Title = Device.DottedPresentationAddressAndName;
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => DevicePicture);

			States = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeStates)
			{
				var stateViewModel = new StateViewModel(state.DriverState, DeviceState.Device);
				States.Add(stateViewModel);
			}

			ParentStates = new List<StateViewModel>();
			foreach (var state in DeviceState.ThreadSafeParentStates)
			{
				var stateViewModel = new StateViewModel(state.DriverState.Clone())
				{
					DeviceName = state.ParentDevice.DottedPresentationAddressAndName
				};
				ParentStates.Add(stateViewModel);
			}
			OnPropertyChanged(() => StateType);
			OnPropertyChanged(() => States);
			OnPropertyChanged(() => ParentStates);
			OnPropertyChanged(() => IsAutomaticOff);

			var property = Device.Properties.FirstOrDefault(x => x.Name == "EnableCountDownTimer");
			if (property != null && property.Value == "1")
			{
				StartTimer(DriverType.RM_1, "RMOn");
				StartTimer(DriverType.MRO, "MRO_On");
				StartTimer(DriverType.MRO_2, "MRO_On");
				StartTimer(DriverType.MDU, "ClapanOn1e");
				StartTimer(DriverType.Valve, "Bolt_On");
				StartTimer(DriverType.MPT, "MPT_On");
			}
		}

		public bool IsAutomaticOff
		{
			get
			{
				foreach (var state in DeviceState.States)
				{
					if (state.DriverState.IsAutomatic && (state.DriverState.Code.Contains("AutoOff") || state.DriverState.Code.Contains("Auto_Off") || state.DriverState.Code.Contains("Auto_off")))
						return true;
				}
				return false;
			}
		}

		void StartTimer(DriverType driverType, string stateCodeName)
		{
			if (Device.Driver.DriverType == driverType)
			{
				var deviceDriverState = DeviceState.ThreadSafeStates.FirstOrDefault(x => x.DriverState.Code == stateCodeName);
				if (deviceDriverState != null)
				{
					if (DateTime.Now > deviceDriverState.Time)
					{
						var timeSpan = DateTime.Now - deviceDriverState.Time;

						var timeoutProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == "AU_Delay");
						if (timeoutProperty == null)
							timeoutProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == "Задержка включения, с");
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
								DeviceControlViewModel.StartTimer(secondsLeft);
								return;
							}
						}
					}
				}
				DeviceControlViewModel.StopTimer();
			}
		}

		void OnParametersChanged()
		{
			OnPropertyChanged(() => Parameters);
		}

		public Brush DevicePicture
		{
			get { return PictureCacheSource.DevicePicture.GetDynamicBrush(Device, AlternativeLibraryDeviceUID); }
		}

		public StateType StateType
		{
			get { return DeviceState.StateType; }
		}
		public List<StateViewModel> States { get; private set; }
		public List<StateViewModel> ParentStates { get; private set; }

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

		public bool CanControl
		{
			get { return ControlDenyMessage == null; }
		}
		public bool CanNotControl
		{
			get { return ControlDenyMessage != null; }
		}

		public string ControlDenyMessage
		{
			get
			{
				var controlProperty = Device.Properties.FirstOrDefault(x => x.Name == "AllowControl");
				if (controlProperty == null || controlProperty.Value != "1")
				{
					return "Управление запрещено настройкой конфигурации";
				}

				if (!FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices))
					return "Управление исполнительными устройствами для данного пользователя запрещено";

#if DEBUG
				return null;
#endif

				if (!ServiceFactory.AppSettings.HasLicenseToControl)
					return "Отсутствует лицензия на управление";

				return null;
			}
		}

		public bool IsControlDevice
		{
			get
			{
				return Device.Driver.HasControlProperties && !FiresecManager.FiresecConfiguration.IsChildMPT(Device);
			}
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.Zone.UID);
		}
		bool CanShowZone()
		{
			return Device.Zone != null;
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Device.UID);
		}
		bool CanShowOnPlan()
		{
			var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => { return x.ElementDevices.Any(y => y.DeviceUID == Device.UID); });
			return plan != null;
		}

		public string PlanName
		{
			get
			{
				var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => { return x.ElementDevices.Any(y => y.DeviceUID == Device.UID); });
				if (plan != null)
					return plan.Caption;
				return null;
			}
		}

		public bool IsOnPlan
		{
			get { return PlanName != null; }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Device.UID.ToString(); }
		}
		#endregion
	}
}