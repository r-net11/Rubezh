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
        public ElementDeviceViewModel()
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);

            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;
        }

        Device _device;
        DeviceState _deviceState;
        ElementDeviceView _elementDeviceView;
        public Guid DeviceUID { get; private set; }

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            _elementDeviceView = new ElementDeviceView()
            {
                DataContext = this,
                Width = elementDevice.Width,
                Height = elementDevice.Height
            };
            //_elementDeviceView._deviceControl
            _elementDeviceView._deviceControl.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseButtonDown);
            Canvas.SetLeft(_elementDeviceView, elementDevice.Left);
            Canvas.SetTop(_elementDeviceView, elementDevice.Top);
            canvas.Children.Add(_elementDeviceView);

            DeviceUID = elementDevice.DeviceUID;
            _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == DeviceUID);
            _deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceUID);
            if (_device != null)
            {
                _elementDeviceView._deviceControl.DriverId = _device.Driver.UID;
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
                _elementDeviceView._selectationRectangle.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsDisabled
        {
            get { return _deviceState.IsDisabled; }
        }

        public event Action Selected;
        void OnPreviewMouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
        }

        public RelayCommand ShowInTreeCommand { get; private set; }
        void OnShowInTree()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.UID);
        }

        public bool CanDisable()
        {
            return _deviceState.CanDisable();
        }

        public RelayCommand DisableCommand { get; private set; }
        void OnDisable()
        {
            if (ServiceFactory.Get<ISecurityService>().Validate())
            {
                _deviceState.ChangeDisabled();
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(_device.UID);
        }

        void OnDeviceStateChanged(Guid deviceUID)
        {
            if (deviceUID == DeviceUID)
            {
                _elementDeviceView._deviceControl.StateType = _deviceState.StateType;
                _elementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
                    from state in _deviceState.States
                    select state.DriverState.Code);

                UpdateTooltip();
            }
        }

        void UpdateTooltip()
        {
            string tooltip = "";
            tooltip = _device.PresentationAddress + " - " + _device.Driver.ShortName + "\n";

            if (_deviceState.ParentStringStates != null)
            {
                foreach (var parentState in _deviceState.ParentStringStates)
                {
                    tooltip += parentState + "\n";
                }
            }

            foreach (var state in _deviceState.States)
            {
                tooltip += state.DriverState.Name + "\n";
            }

            if (_deviceState.Parameters != null)
                foreach (var parameter in _deviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
                            continue;
                        tooltip += parameter.Caption + " - " + parameter.Value + "\n";
                    }
                }

            if (_elementDeviceView != null)
                _elementDeviceView.ToolTip = tooltip;
        }
    }
}