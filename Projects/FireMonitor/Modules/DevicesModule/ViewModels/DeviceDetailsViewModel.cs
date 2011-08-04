using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public DeviceDetailsViewModel(string deviceId)
        {
            _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
            _deviceControlViewModel = new DeviceControlViewModel(_device);
            CloseCommand = new RelayCommand(OnClosing);
            Title = _device.Driver.ShortName + " " + _device.DottedAddress;
        }

        Device _device;
        DeviceControls.DeviceControl _deviceControl;
        DeviceControlViewModel _deviceControlViewModel;

        public Driver Driver
        {
            get { return _device.Driver; }
        }

        void deviceState_StateChanged()
        {
            DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);

            if (_deviceControl != null)
            {
                _deviceControl.StateId = deviceState.State.Id.ToString();
            }

            OnPropertyChanged("DeviceControlContent");
        }

        public object DeviceControlContent
        {
            get
            {
                _deviceControl = new DeviceControls.DeviceControl();
                _deviceControl.DriverId = _device.Driver.Id;

                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                _deviceControl.StateId = deviceState.State.Id.ToString();

                _deviceControl.Width = 50;
                _deviceControl.Height = 50;

                return _deviceControl;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (_device.Parent != null)
                {
                    return _device.Parent.Driver.Name;
                }
                return null;
            }
        }

        public string PresentationZone
        {
            get { return _device.GetPersentationZone(); }
        }

        public ObservableCollection<string> SelfStates
        {
            get
            {
                ObservableCollection<string> selfStates = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                    foreach (var innerState in deviceState.InnerStates)
                    {
                        if (innerState.IsActive)
                        selfStates.Add(innerState.Name);
                    }
                return selfStates;
            }
        }

        public ObservableCollection<string> ParentStates
        {
            get
            {
                ObservableCollection<string> parentStates = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.ParentStringStates != null)
                    foreach (var parentState in deviceState.ParentStringStates)
                    {
                        parentStates.Add(parentState);
                    }
                return parentStates;
            }
        }

        public ObservableCollection<string> Parameters
        {
            get
            {
                ObservableCollection<string> parameters = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.Parameters != null)
                    foreach (var parameter in deviceState.Parameters)
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

        public State State
        {
            get
            {
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                return deviceState.State;
            }
        }

        public event Action Closing;
        void OnClosing()
        {
            if (Closing != null)
                Closing();
        }

        public RelayCommand CloseCommand { get; private set; }
    }

}
