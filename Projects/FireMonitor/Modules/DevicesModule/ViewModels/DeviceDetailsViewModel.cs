using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using Firesec;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecClient.Models;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public DeviceDetailsViewModel()
        {
            Title = "Свойства устройства";
        }

        Device _device;
        DeviceControls.DeviceControl _deviceControl;
        public DeviceControlViewModel DeviceControlViewModel { get; set; }

        public void Initialize(string deviceId)
        {
            _device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == deviceId);
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
            DeviceControlViewModel = new DeviceControlViewModel(_device);
        }

        public Driver Driver
        {
            get { return _device.Driver; }
        }

        void deviceState_StateChanged()
        {
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);

            if (_deviceControl != null)
            {
                _deviceControl.State = deviceState.State.Id.ToString();
            }

            OnPropertyChanged("DeviceControlContent");
        }

        public object DeviceControlContent
        {
            get
            {
                _deviceControl = new DeviceControls.DeviceControl();

                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                _deviceControl.State = deviceState.State.Id.ToString();

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
            get { return _device.PresentationZone; }
        }

        public ObservableCollection<string> SelfStates
        {
            get
            {
                ObservableCollection<string> selfStates = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.SelfStates != null)
                    foreach (var selfState in deviceState.SelfStates)
                    {
                        selfStates.Add(selfState);
                    }
                return selfStates;
            }
        }

        public ObservableCollection<string> ParentStates
        {
            get
            {
                ObservableCollection<string> parentStates = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
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
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
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
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                return deviceState.State;
            }
        }
    }
}
