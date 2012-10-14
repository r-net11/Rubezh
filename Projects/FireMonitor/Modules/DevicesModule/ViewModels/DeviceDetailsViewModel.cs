using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace DevicesModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }
		public DeviceControlViewModel DeviceControlViewModel { get; private set; }
		DeviceControls.DeviceControl _deviceControl;
		private Guid _guid;

		public DeviceDetailsViewModel(Device device)
		{
			Device = device;
			_guid = device.UID;
			DeviceState = Device.DeviceState;
			DeviceControlViewModel = new DeviceControlViewModel(Device);
			DeviceState.StateChanged += new Action(OnStateChanged);
			DeviceState.ParametersChanged += new Action(OnParametersChanged);
			OnStateChanged();

			Title = Device.Driver.ShortName + " " + Device.DottedAddress;
			TopMost = true;
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		void OnStateChanged()
		{
			if (DeviceState != null && _deviceControl != null)
				_deviceControl.StateType = DeviceState.StateType;
			OnPropertyChanged("DeviceControlContent");
			//OnPropertyChanged("DeviceState");

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
			OnPropertyChanged("States");
			OnPropertyChanged("ParentStringStates");

			StartTimer(DriverType.RM_1, "RMOn");
			StartTimer(DriverType.MRO_2, "MRO_On");
			StartTimer(DriverType.MDU, "ClapanOn1e");
			StartTimer(DriverType.Valve, "Bolt_On");
			StartTimer(DriverType.MPT, "MPT_On");
		}

		void StartTimer(DriverType driverType, string stateCodeName)
		{
			if (Device.Driver.DriverType == driverType)
			{
				var deviceDriverState = DeviceState.States.FirstOrDefault(x => x.DriverState.Code == stateCodeName);
				if (deviceDriverState != null)
				{
					if (DateTime.Now > deviceDriverState.Time)
					{
						var timeSpan = deviceDriverState.Time - DateTime.Now;

						var timeoutProperty = Device.Properties.FirstOrDefault(x => x.Name == "Timeout");
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

							int secondsLeft = timeout - timeSpan.Value.Seconds;
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
			OnPropertyChanged("Parameters");
		}

		public object DeviceControlContent
		{
			get
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == Device.Driver.UID);
				if (libraryDevice == null)
				{
					return null;
				}
				if (DeviceState != null)
				{
					_deviceControl = new DeviceControls.DeviceControl()
					{
						DriverId = Device.Driver.UID,
						Width = 50,
						Height = 50,
						StateType = DeviceState.StateType,
						AdditionalStateCodes = new List<string>(
							from state in DeviceState.States
							select state.DriverState.Code)
					};
					_deviceControl.Update();
				}

				return _deviceControl;
			}
		}

		public List<StateViewModel> States { get; private set; }
		public List<StateViewModel> ParentStates { get; private set; }
		//public List<DeviceDriverState> States
		//{
		//    get { return DeviceState.States; }
		//}

		//public List<string> ParentStringStates
		//{
		//    get { return DeviceState.ParentStringStates; }
		//}

		public List<string> Parameters
		{
			get
			{
				var parameters = new List<string>();
				if (DeviceState != null && DeviceState.Parameters != null)
				{
					foreach (var parameter in DeviceState.Parameters)
					{
						if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == "<NULL>")
							continue;
						if (!parameter.Visible)
							continue;
						parameters.Add(parameter.Caption + ": " + parameter.Value);
					}
				}
				return parameters;
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
                //if (!ServiceFactory.AppSettings.HasLicenseToControl)
                //    return "Отсутствует лицензия на управление";

                var controlProperty = Device.Properties.FirstOrDefault(x => x.Name == "AllowControl");
                if (controlProperty != null)
                {
                    if (controlProperty.Value != "1")
                        return "Управление запрещено настройкой конфигурации";
                }

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

		bool _isControlTabSelected;
		public bool IsControlTabSelected
		{
			get { return _isControlTabSelected; }
			set
			{
				_isControlTabSelected = value;
				OnPropertyChanged("IsControlTabSelected");
			}
		}

		//public void StartValveTimer(int timeLeft)
		//{
		//    IsControlTabSelected = true;
		//    ValveControlViewModel.StartTimer(timeLeft);
		//}

		#region IWindowIdentity Members
		public Guid Guid
		{
			get { return _guid; }
		}
		#endregion
	}
}