using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }
		public DeviceControlViewModel DeviceControlViewModel { get; private set; }
		public ValveControlViewModel ValveControlViewModel { get; private set; }
		DeviceControls.DeviceControl _deviceControl;
		private Guid _guid;

		public DeviceDetailsViewModel(Guid deviceUID)
		{
			_guid = deviceUID;
			Device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			DeviceState = Device.DeviceState;
			if (DeviceState != null)
				DeviceState.StateChanged += new Action(deviceState_StateChanged);
			DeviceControlViewModel = new DeviceControlViewModel(Device);
			ValveControlViewModel = new ValveControlViewModel(Device);

			Title = Device.Driver.ShortName + " " + Device.DottedAddress;
			TopMost = true;
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		void deviceState_StateChanged()
		{
			if (DeviceState != null && _deviceControl != null)
				_deviceControl.StateType = DeviceState.StateType;
			OnPropertyChanged("DeviceControlContent");
		}

		public object DeviceControlContent
		{
			get
			{
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
						parameters.Add(parameter.Caption + " - " + parameter.Value);
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

		public void StartValveTimer(int timeLeft)
		{
			IsControlTabSelected = true;
			ValveControlViewModel.StartTimer(timeLeft);
		}

		#region IWindowIdentity Members
		public Guid Guid
		{
			get { return _guid; }
		}
		#endregion
	}
}