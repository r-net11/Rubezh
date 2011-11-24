using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceView ElementDeviceView { get; private set; }
        Device Device;
        DeviceState DeviceState;
        public Guid DeviceUID { get; private set; }

        public ElementDeviceViewModel(ElementDevice elementDevice)
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;

            ElementDeviceView = new ElementDeviceView();

            DeviceUID = elementDevice.DeviceUID;
            Device = elementDevice.Device;
            DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceUID);
            if (Device != null)
            {
                ElementDeviceView._deviceControl.DriverId = Device.Driver.UID;
                OnDeviceStateChanged(DeviceUID);
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                ElementDeviceView._selectationRectangle.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsDisabled
        {
            get { return DeviceState.IsDisabled; }
        }

        public RelayCommand ShowInTreeCommand { get; private set; }
        void OnShowInTree()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Device.UID);
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

        void OnDeviceStateChanged(Guid deviceUID)
        {
            if (deviceUID == DeviceUID)
            {
                ElementDeviceView._deviceControl.StateType = DeviceState.StateType;
                ElementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
                    from state in DeviceState.States
                    select state.DriverState.Code);

                UpdateTooltip();
            }
        }

        string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                OnPropertyChanged("ToolTip");
            }
        }

        void UpdateTooltip()
        {
            string tooltip = "";
            tooltip = Device.PresentationAddress + " - " + Device.Driver.ShortName + "\n";

            if (DeviceState.ParentStringStates != null)
            {
                foreach (var parentState in DeviceState.ParentStringStates)
                {
                    tooltip += parentState + "\n";
                }
            }

            foreach (var state in DeviceState.States)
            {
                tooltip += state.DriverState.Name + "\n";
            }

            if (DeviceState.Parameters != null)
                foreach (var parameter in DeviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
                            continue;
                        tooltip += parameter.Caption + " - " + parameter.Value + "\n";
                    }
                }

            ToolTip = tooltip;
        }
    }
}