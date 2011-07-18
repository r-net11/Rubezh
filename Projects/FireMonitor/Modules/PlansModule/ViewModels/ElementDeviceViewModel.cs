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
using Firesec;
using System.Diagnostics;
using FiresecClient.Models;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        Device _device;
        DeviceState _deviceState;
        DeviceControls.DeviceControl _deviceControl;
        Rectangle _mouseOverRectangle;
        Rectangle _selectationRectangle;
        ElementDevice _elementDevice;
        Canvas _tooltipCanvas;

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            _elementDevice = elementDevice;

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, elementDevice.Left);
            Canvas.SetTop(innerCanvas, elementDevice.Top);
            canvas.Children.Add(innerCanvas);

            _deviceControl = new DeviceControls.DeviceControl();
            _deviceControl.Width = elementDevice.Width;
            _deviceControl.Height = elementDevice.Height;
            innerCanvas.Children.Add(_deviceControl);

            _mouseOverRectangle = new Rectangle();
            _mouseOverRectangle.Width = elementDevice.Width;
            _mouseOverRectangle.Height = elementDevice.Height;
            _mouseOverRectangle.Stroke = Brushes.Red;
            _mouseOverRectangle.StrokeThickness = 0;
            innerCanvas.Children.Add(_mouseOverRectangle);

            _selectationRectangle = new Rectangle();
            _selectationRectangle.Width = elementDevice.Width;
            _selectationRectangle.Height = elementDevice.Height;
            _selectationRectangle.Stroke = Brushes.Orange;
            _selectationRectangle.StrokeThickness = 0;
            innerCanvas.Children.Add(_selectationRectangle);

            IsSelected = false;

            _device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == elementDevice.Id);
            _deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == elementDevice.Id);
            if (_device != null)
            {
                _deviceControl.DriverId = _device.Driver.Id;
                AddTooltipCanvas(elementDevice, canvas);
                OnDeviceStateChanged(elementDevice.Id);
            }

            XDeviceView xDeviceView = new XDeviceView();
            xDeviceView.DataContext = this;
            XText = "_";
            innerCanvas.Children.Add(xDeviceView);
        }

        string _xText;
        public string XText
        {
            get { return _xText; }
            set
            {
                _xText = value;
                OnPropertyChanged("XText");
            }
        }

        void AddTooltipCanvas(ElementDevice elementDevice, Canvas canvas)
        {
            _tooltipCanvas = new Canvas();
            _tooltipCanvas.Width = elementDevice.Width;
            _tooltipCanvas.Height = elementDevice.Height;
            Canvas.SetLeft(_tooltipCanvas, elementDevice.Left);
            Canvas.SetTop(_tooltipCanvas, elementDevice.Top);
            _tooltipCanvas.Background = Brushes.White;
            _tooltipCanvas.Opacity = 0.01;

            ContextMenu contextMenu = new ContextMenu();
            
            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "Показать в дереве";
            menuItem1.Click += new System.Windows.RoutedEventHandler(OnShowInTree);
            contextMenu.Items.Add(menuItem1);

            if (_device.Driver.CanDisable)
            {
                MenuItem menuItem2 = new MenuItem();
                menuItem2.Header = "Отключить";
                menuItem2.Click += new System.Windows.RoutedEventHandler(OnDisable);
                contextMenu.Items.Add(menuItem2);
            }

            MenuItem menuItem3 = new MenuItem();
            menuItem3.Header = "Свойства";
            menuItem3.Click += new System.Windows.RoutedEventHandler(OnShowProperties);
            contextMenu.Items.Add(menuItem3);

            _tooltipCanvas.ContextMenu = contextMenu;

            _tooltipCanvas.MouseEnter += new System.Windows.Input.MouseEventHandler(OnMouseEnter);
            _tooltipCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(OnMouseLeave);
            _tooltipCanvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);

            canvas.Children.Add(_tooltipCanvas);
        }

        void OnShowInTree(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.Id);
        }

        void OnDisable(object sender, System.Windows.RoutedEventArgs e)
        {
            _deviceState.ChangeDisabled();
        }

        void OnShowProperties(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(_device.Id);
        }

        void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 1;
        }

        void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 0;
        }

        public string DeviceId
        {
            get
            {
                return _elementDevice.Id;
            }
        }

        public event Action Selected;

        void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                _selectationRectangle.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        void OnDeviceStateChanged(string id)
        {
            if (id == _elementDevice.Id)
            {
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == id);
                Device device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == id);
                _deviceControl.State = deviceState.State.Id.ToString();

                string tooltip = "";
                tooltip = device.PresentationAddress + " - " + _device.Driver.ShortName + "\n";

                if (deviceState.ParentStringStates != null)
                    foreach (var parentState in deviceState.ParentStringStates)
                    {
                        tooltip += parentState + "\n";
                    }

                if (deviceState.SelfStates != null)
                    foreach (var selfState in deviceState.SelfStates)
                    {
                        tooltip += selfState + "\n";
                    }

                if (deviceState.Parameters != null)
                    foreach (var parameter in deviceState.Parameters)
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

                _tooltipCanvas.ToolTip = tooltip;
            }
        }
    }
}
