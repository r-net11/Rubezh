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

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public DeviceDetailsViewModel()
        {
            Title = "Свойства устройства";
        }

        FiresecClient.Device _device;
        DeviceControls.DeviceControl _deviceControl;

        public void Initialize(string deviceId)
        {
            _device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == deviceId);
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
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
                _deviceControl.DriverId = _device.DriverId;

                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                _deviceControl.State = deviceState.State.Id.ToString();

                _deviceControl.Width = 50;
                _deviceControl.Height = 50;

                return _deviceControl;
            }
        }

        public string DeviceCategory
        {
            get
            {
                switch (_device.Driver.cat)
                {
                    case "0":
                        return "Прочие устройства";

                    case "1":
                        return "Приборы";

                    case "2":
                        return "Датчики";

                    case "3":
                        return "ИУ";

                    case "4":
                        return "Сеть передачи данных";

                    case "5":
                        return "Не указано";

                    case "6":
                        return "Удаленный сервер";
                }

                return "";
            }
        }

        public string DeviceType
        {
            get
            {
                if (_device.Driver.options != null)
                {
                    if (_device.Driver.options.Contains("FireOnly"))
                        return "пожарный";

                    if (_device.Driver.options.Contains("SecOnly"))
                        return "охранный";

                    if (_device.Driver.options.Contains("TechOnly"))
                        return "технологический";
                }
                
                return "охранно-пожарный";
            }
        }

        public bool HasImage
        {
            get
            {
                return (ImageSource != @"C:/Program Files/Firesec/Icons/Device_Device.ico");
            }
        }

        public string ImageSource
        {
            get
            {
                string ImageName;
                if (!string.IsNullOrEmpty(_device.Driver.dev_icon))
                {
                    ImageName = _device.Driver.dev_icon;
                }
                else
                {
                    var metadataClass = FiresecManager.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == _device.Driver.clsid);
                    ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
                }

                return @"C:/Program Files/Firesec/Icons/" + ImageName + ".ico";
                //return @"pack://application:,,,/Icons/" + ImageName + ".ico";
            }
        }

        public string DriverName
        {
            get
            {
                return _device.Driver.name;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (_device.Parent != null)
                {
                    return _device.Parent.Driver.name;
                }
                return null;
            }
        }

        public bool IsZoneDevice
        {
            get { return _device.IsZoneDevice; }
        }

        public bool IsZoneLogicDevice
        {
            get { return _device.IsZoneLogicDevice; }
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

        public bool CanControl
        {
            get
            {
                var driverName = DriversHelper.GetDriverNameById(_device.Driver.id);
                return (driverName == "Задвижка");
            }
        }
    }
}
