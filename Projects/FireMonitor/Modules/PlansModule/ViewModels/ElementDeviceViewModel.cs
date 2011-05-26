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

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        Device _device;
        Firesec.Metadata.drvType _driver;
        DeviceControls.DeviceControl _deviceControl;
        Rectangle _mouseOverRectangle;
        Rectangle _selectationRectangle;
        ElementDevice _elementDevice;
        Canvas _tooltipCanvas;

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            _elementDevice = elementDevice;

            _device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == elementDevice.Id);
            if (_device == null)
                return;
            _driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == _device.DriverId);

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, elementDevice.Left);
            Canvas.SetTop(innerCanvas, elementDevice.Top);
            canvas.Children.Add(innerCanvas);

            _deviceControl = new DeviceControls.DeviceControl();
            _deviceControl.DriverId = _device.DriverId;

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

            AddTooltipCanvas(elementDevice, canvas);

            IsSelected = false;
            OnDeviceStateChanged(elementDevice.Id);
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
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Показать в дереве";
            menuItem.Click += new System.Windows.RoutedEventHandler(menuItem_Click);
            contextMenu.Items.Add(menuItem);
            _tooltipCanvas.ContextMenu = contextMenu;

            _tooltipCanvas.MouseEnter += new System.Windows.Input.MouseEventHandler(OnMouseEnter);
            _tooltipCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(OnMouseLeave);
            _tooltipCanvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);

            canvas.Children.Add(_tooltipCanvas);
        }

        void menuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.Id);
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
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == id);
                Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == id);
                _deviceControl.State = deviceState.State.Id.ToString();

                string tooltip = "";
                tooltip = device.Address + " - " + _driver.shortName + "\n";

                if (deviceState.ParentStringStates != null)
                    foreach (string parentState in deviceState.ParentStringStates)
                    {
                        tooltip += parentState + "\n";
                    }

                if (deviceState.SelfStates != null)
                    foreach (string selfState in deviceState.SelfStates)
                    {
                        tooltip += selfState + "\n";
                    }

                if (deviceState.Parameters != null)
                    foreach (Parameter parameter in deviceState.Parameters)
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
