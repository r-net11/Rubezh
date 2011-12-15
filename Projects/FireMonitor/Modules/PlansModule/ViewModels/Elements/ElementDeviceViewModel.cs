using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using DeviceControls;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using PlansModule.Events;
using System.Diagnostics;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceView ElementDeviceView { get; private set; }
        ElementDevice ElementDevice;
        public Guid DeviceUID { get; private set; }
        Device Device;
        DeviceState DeviceState;

        public ElementDeviceViewModel(ElementDevice elementDevice)
        {
            ElementDevice = elementDevice;
            DeviceUID = elementDevice.DeviceUID;
            Device = elementDevice.Device;
            DeviceState = elementDevice.DeviceState;
        }

        object locker = new object();

        public void DrawElementDevice()
        {
            lock (locker)
            {
                if (ElementDeviceView != null)
                    return;

                ElementDeviceView = new ElementDeviceView()
                {
                    DataContext = this
                };
            }
            ElementDeviceView._deviceControl.IsManualUpdate = true;

            ElementDeviceView.Width = ElementDevice.Width;
            ElementDeviceView.Height = ElementDevice.Height;
            Canvas.SetLeft(ElementDeviceView, ElementDevice.Left);
            Canvas.SetTop(ElementDeviceView, ElementDevice.Top);

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
                DrawElementDevice();
                ElementDeviceView._selectationRectangle.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");

                if (value) ElementDeviceView.Flush();
                {
                }
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
                ElementDeviceView._deviceControl.Update();

                //DeviceControl.StateType = DeviceState.StateType;
                //DeviceControl.AdditionalStateCodes = new List<string>(
                //    from state in DeviceState.States
                //    select state.DriverState.Code);
                //DeviceControl.Update();

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