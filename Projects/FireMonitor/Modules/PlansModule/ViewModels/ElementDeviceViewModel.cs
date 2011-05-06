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
using Infrastructure.Events;
using Firesec;
using System.Diagnostics;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        Device device;
        Firesec.Metadata.drvType Driver;
        DeviceControls.DeviceControl deviceControl;
        Rectangle mouseOverRectangle;
        Rectangle selectationRectangle;
        public ElementDevice elementDevice;

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            this.elementDevice = elementDevice;

            if (FiresecManager.CurrentConfiguration.AllDevices.Any(x => x.Path == elementDevice.Path))
            {
                device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == elementDevice.Path);
                Driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
            }

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, elementDevice.Left);
            Canvas.SetTop(innerCanvas, elementDevice.Top);
            canvas.Children.Add(innerCanvas);

            deviceControl = new DeviceControls.DeviceControl();
            deviceControl.DriverId = device.DriverId;

            //deviceControl.ToolTip = Name;
            deviceControl.Width = elementDevice.Width;
            deviceControl.Height = elementDevice.Height;
            innerCanvas.Children.Add(deviceControl);

            mouseOverRectangle = new Rectangle();
            mouseOverRectangle.Width = elementDevice.Width;
            mouseOverRectangle.Height = elementDevice.Height;
            mouseOverRectangle.Stroke = Brushes.Red;
            mouseOverRectangle.StrokeThickness = 0;
            innerCanvas.Children.Add(mouseOverRectangle);

            selectationRectangle = new Rectangle();
            selectationRectangle.Width = elementDevice.Width;
            selectationRectangle.Height = elementDevice.Height;
            selectationRectangle.Stroke = Brushes.Orange;
            selectationRectangle.StrokeThickness = 0;
            innerCanvas.Children.Add(selectationRectangle);

            innerCanvas.MouseEnter += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseEnter);
            innerCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseLeave);
            innerCanvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(innerCanvas_PreviewMouseLeftButtonDown);

            IsSelected = false;
            OnDeviceStateChanged(elementDevice.Path);
        }

        void innerCanvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseOverRectangle.StrokeThickness = 1;
        }

        void innerCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseOverRectangle.StrokeThickness = 0;
        }

        public event Action Selected;

        void innerCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                selectationRectangle.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Publish(device.Path);
        }

        public string Name
        {
            get { return Driver.shortName; }
        }

        public string Address
        {
            get { return device.Address; }
        }

        void OnDeviceStateChanged(string path)
        {
            if (path == elementDevice.Path)
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
                deviceControl.StateId = StateHelper.NameToPriority(deviceState.State).ToString();
            }
        }
    }
}
