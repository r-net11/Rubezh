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
        Firesec.Metadata.drvType _driver;
        DeviceControls.DeviceControl _deviceControl;

        public void Initialize(string deviceId)
        {
            _device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == deviceId);
            _driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == _device.DriverId);
            DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
        }

        void deviceState_StateChanged()
        {
            DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);

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

                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
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
                switch (_driver.cat)
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
                if (!string.IsNullOrEmpty(_driver.dev_icon))
                {
                    ImageName = _driver.dev_icon;
                }
                else
                {
                    Firesec.Metadata.classType metadataClass = FiresecManager.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == _driver.clsid);
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
                return _driver.name;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (_device.Parent != null)
                {
                    var parentDriver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == _device.Parent.DriverId);
                    return parentDriver.name;
                }
                return null;
            }
        }

        public bool IsZoneDevice
        {
            get
            {
                if ((_driver.minZoneCardinality == "0") && (_driver.maxZoneCardinality == "0"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                if ((_driver.options != null) && (_driver.options.Contains("ExtendedZoneLogic")))
                {
                    return true;
                }
                return false;
            }
        }

        public string PresentationZone
        {
            get
            {
                if (IsZoneDevice)
                {
                    if (string.IsNullOrEmpty(_device.ZoneNo))
                        return "";

                    Zone zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == _device.ZoneNo);
                    return _device.ZoneNo + "." + zone.Name;
                }
                if (IsZoneLogicDevice)
                {
                    return ZoneLogicToText.Convert(_device.ZoneLogic);
                }
                return "";
            }
        }

        public ObservableCollection<string> SelfStates
        {
            get
            {
                ObservableCollection<string> selfStates = new ObservableCollection<string>();
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.SelfStates != null)
                    foreach (string selfState in deviceState.SelfStates)
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
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.ParentStringStates != null)
                    foreach (string parentState in deviceState.ParentStringStates)
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
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.Parameters != null)
                    foreach (Parameter parameter in deviceState.Parameters)
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
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                return deviceState.State;
            }
        }

        public bool CanControl
        {
            get
            {
                var driverName = DriversHelper.GetDriverNameById(_driver.id);
                return (driverName == "Задвижка");
            }
        }
    }
}
