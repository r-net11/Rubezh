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

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        DeviceControls.DeviceControl deviceControl;
        Rectangle mouseOverRectangle;
        Rectangle selectationRectangle;
        public ElementDevice elementDevice;
        PlanViewModel planViewModel;

        bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (value)
                {
                    selectationRectangle.StrokeThickness = 1;
                }
                else
                {
                    selectationRectangle.StrokeThickness = 0;
                }
            }
        }

        public void Initialize(ElementDevice elementDevice, Canvas canvas, PlanViewModel planViewModel)
        {
            this.planViewModel = planViewModel;
            this.elementDevice = elementDevice;

            if (FiresecManager.CurrentConfiguration.AllDevices.Any(x => x.Path == elementDevice.Path))
            {
                device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == elementDevice.Path);
                Firesec.Metadata.drvType Driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                Name = Driver.shortName;
            }

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, elementDevice.Left);
            Canvas.SetTop(innerCanvas, elementDevice.Top);
            canvas.Children.Add(innerCanvas);

            deviceControl = new DeviceControls.DeviceControl();
            string deviceName = DriversHelper.GetDriverNameById(device.DriverId);
            deviceName = deviceName.Replace("//", "/");
            deviceControl.DriverId = deviceName;
            //deviceControl.ToolTip = Name;
            deviceControl.Width = elementDevice.Width;
            deviceControl.Height = elementDevice.Height;
            innerCanvas.Children.Add(deviceControl);

            mouseOverRectangle = new Rectangle();
            mouseOverRectangle.Width = elementDevice.Width;
            mouseOverRectangle.Height = elementDevice.Height;
            mouseOverRectangle.Stroke = Brushes.Red;
            innerCanvas.Children.Add(mouseOverRectangle);

            selectationRectangle = new Rectangle();
            selectationRectangle.Width = elementDevice.Width;
            selectationRectangle.Height = elementDevice.Height;
            selectationRectangle.Stroke = Brushes.Orange;
            innerCanvas.Children.Add(selectationRectangle);

            innerCanvas.MouseEnter += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseEnter);
            innerCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseLeave);
            innerCanvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(innerCanvas_PreviewMouseLeftButtonDown);

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

        void innerCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<PlanDeviceSelectedEvent>().Publish(elementDevice.Path);
        }


        Device device;

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Publish(device.Path);
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public string State { get; set; }

        void OnDeviceStateChanged(string path)
        {
            if (path == elementDevice.Path)
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
                State = deviceState.State;
                StateType stateType = StateHelper.NameToType(deviceState.State);

                if (deviceState.State == "Неопределено")
                    deviceControl.StateId = "Неизвестно";
                else
                deviceControl.StateId = deviceState.State;

                planViewModel.UpdateSelfState();
            }
        }
    }
}
