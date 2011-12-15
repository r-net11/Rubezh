using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            DeviceUID = elementDevice.DeviceUID;
            Device = elementDevice.Device;

            Action initializer = new Action(Initialize);
            IAsyncResult result = initializer.BeginInvoke(null, null);

            ElementDeviceView = new ElementDeviceView(); //TODO: ~25 %
            if (Device != null)
            {
                ElementDeviceView._deviceControl.DriverId = Device.Driver.UID;
            }
            ElementDeviceView._deviceControl.StateType = DeviceState.StateType;
            ElementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
                from state in DeviceState.States
                select state.DriverState.Code);

            initializer.EndInvoke(result);
        }

        void Initialize()
        {
            DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceUID); // TODO: ~20%
            if (Device != null)
            {
                UpdateTooltip();
            }
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;
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

                if (value) ElementDeviceView.Flush();
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
                DeviceState.ChangeDisabled();
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
            var tooltipBuilder = new StringBuilder();
            tooltipBuilder.Append(Device.PresentationAddress);
            tooltipBuilder.Append(" - ");
            tooltipBuilder.Append(Device.Driver.ShortName);
            tooltipBuilder.Append("\n");

            if (DeviceState.ParentStringStates != null)
            {
                foreach (var parentState in DeviceState.ParentStringStates)
                {
                    tooltipBuilder.AppendLine(parentState);
                }
            }

            foreach (var state in DeviceState.States)
            {
                tooltipBuilder.AppendLine(state.DriverState.Name);
            }

            if (DeviceState.Parameters != null)
            {
                var nullString = "<NULL>";
                foreach (var parameter in DeviceState.Parameters.Where(x => x.Visible && string.IsNullOrEmpty(x.Value) == false && x.Value != nullString))
                {
                    tooltipBuilder.Append(parameter.Caption);
                    tooltipBuilder.Append(" - ");
                    tooltipBuilder.AppendLine(parameter.Value);
                }
            }

            ToolTip = tooltipBuilder.ToString();
        }
    }
}