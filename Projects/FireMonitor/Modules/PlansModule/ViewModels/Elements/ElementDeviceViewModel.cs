using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using PlansModule.Events;
using PlansModule.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Events;
using System.Diagnostics;
using FiresecAPI.Models;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);

            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        Device _device;
        DeviceState _deviceState;
        ElementDeviceView _elementDeviceView;

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            _elementDeviceView = new ElementDeviceView();
            _elementDeviceView.DataContext = this;
            _elementDeviceView._deviceControl.Width = elementDevice.Width;
            _elementDeviceView._deviceControl.Height = elementDevice.Height;
            _elementDeviceView._deviceControl.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseButtonDown);
            Canvas.SetLeft(_elementDeviceView, elementDevice.Left);
            Canvas.SetTop(_elementDeviceView, elementDevice.Top);
            canvas.Children.Add(_elementDeviceView);

            DeviceId = elementDevice.Id;
            _device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == DeviceId);
            _deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == DeviceId);
            if (_device != null)
            {
                _elementDeviceView._deviceControl.DriverId = _device.Driver.Id;
                OnDeviceStateChanged(elementDevice.Id);
            }
        }

        public string DeviceId { get; private set; }

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
            get
            {
                return _deviceState.IsDisabled;
            }
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
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.Id);
        }

        public bool CanDisable(object obj)
        {
            return _deviceState.CanDisable;
        }

        public RelayCommand DisableCommand { get; private set; }
        void OnDisable()
        {
            bool result = ServiceFactory.Get<ISecurityService>().Check();
            if (result)
            {
                _deviceState.ChangeDisabled();
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(_device.Id);
        }

        void OnDeviceStateChanged(string id)
        {
            if (id == DeviceId)
            {
                _elementDeviceView._deviceControl.StateId = _deviceState.State.Id.ToString();
                _elementDeviceView._deviceControl.AdditionalStates = new List<string>(
                    from state in _deviceState.InnerStates
                    where state.IsActive
                    select state.Id);

                UpdateTooltip();
            }
        }

        void UpdateTooltip()
        {
            string tooltip = "";
            tooltip = _device.PresentationAddress + " - " + _device.Driver.ShortName + "\n";

            if (_deviceState.ParentStringStates != null)
                foreach (var parentState in _deviceState.ParentStringStates)
                {
                    tooltip += parentState + "\n";
                }

            if (_deviceState.SelfStates != null)
                foreach (var selfState in _deviceState.SelfStates)
                {
                    tooltip += selfState + "\n";
                }

            if (_deviceState.Parameters != null)
                foreach (var parameter in _deviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        if (string.IsNullOrEmpty(parameter.Value))
                            continue;
                        if (parameter.Value == "<NULL>")
                            continue;
                        tooltip += parameter.Caption + " - " + parameter.Value + "\n";
                    }
                }

            if (_elementDeviceView != null)
                _elementDeviceView.ToolTip = tooltip;
        }
    }
}
