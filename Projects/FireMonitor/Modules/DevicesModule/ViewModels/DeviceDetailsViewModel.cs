using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent, IDialogContentGuid
    {
        public Device Device { get; private set; }
        public DeviceState DeviceState { get; private set; }
        public DeviceControlViewModel DeviceControlViewModel { get; private set; }
        DeviceControls.DeviceControl _deviceControl;
		private Guid _guid;

        public DeviceDetailsViewModel(Guid deviceUID)
        {
			_guid = deviceUID;
            Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            if (DeviceState != null)
                DeviceState.StateChanged += new Action(deviceState_StateChanged);
            DeviceControlViewModel = new DeviceControlViewModel(Device);

            Title = Device.Driver.ShortName + " " + Device.DottedAddress;
        }

        public string PresentationZone
        {
            get { return Device.GetPersentationZone(); }
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
            get
            {
                return (Device.Driver.CanControl && ServiceFactory.AppSettings.CanControl);
            }
        }

        bool _isValveControlSelected;
        public bool IsValveControlSelected
        {
            get { return _isValveControlSelected; }
            set
            {
                _isValveControlSelected = value;
                OnPropertyChanged("IsValveControlSelected");
            }
        }

        public void StartValveTimer(int timeLeft)
        {
            IsValveControlSelected = true;
            DeviceControlViewModel.StartTimer(timeLeft);
        }

		#region IDialogContentGuid Members

		public Guid Guid
		{
			get { return _guid; }
		}

		#endregion
	}
}