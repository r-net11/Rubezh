using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using DevicesModule.Views;
using System.Diagnostics;
using Infrastructure.Common;
using Infrastructure.Events;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public DeviceViewModel()
        {
            ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowOnPlan);
            ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
            DisableCommand = new RelayCommand(OnDisable, CanDisable);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
            _deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == Device.Id);
            UpdateParameters();
        }

        public Device Device { get; private set; }
        DeviceState _deviceState;

        public Driver Driver
        {
            get { return Device.Driver; }
        }

        public void UpdateParameters()
        {
            Update();

            if (_deviceState.Parameters != null)
            {
                foreach (var parameter in _deviceState.Parameters)
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

        public ObservableCollection<string> SelfStates
        {
            get
            {
                ObservableCollection<string> selfStates = new ObservableCollection<string>();
                if (_deviceState.SelfStates != null)
                    foreach (var selfState in _deviceState.SelfStates)
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
                if (_deviceState.ParentStringStates != null)
                    foreach (var parentState in _deviceState.ParentStringStates)
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
                if (_deviceState.Parameters != null)
                    foreach (var parameter in _deviceState.Parameters)
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
                return _deviceState.State;
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

        public bool IsDisabled
        {
            get
            {
                return _deviceState.IsDisabled;
            }
        }

        public void Update()
        {
            OnPropertyChanged("State");
        }

        public bool CanShowOnPlan(object obj)
        {
            return true;
        }

        public RelayCommand ShowPlanCommand { get; private set; }
        void OnShowPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Device.Id);
        }

        public bool CanShowZone(object obj)
        {
            return ((Device.Driver.IsZoneDevice) && (string.IsNullOrEmpty(this.Device.ZoneNo) == false));
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneNo);
        }

        public bool CanDisable(object obj)
        {
            return _deviceState.CanDisable;
        }

        public RelayCommand DisableCommand { get; private set; }
        void OnDisable()
        {
            //SoundService.Play("Sound1.wav");
            //SoundService.Beep();

            bool result = ServiceFactory.Get<ISecurityService>().Check();
            if (result)
            {
                _deviceState.ChangeDisabled();
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(Device.Id);
        }
    }
}
