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
using ClientApi;
using Infrastructure.Events;
using FiresecMetadata;

namespace PlansModule.ViewModels
{
    public class ElementDeviceViewModel : BaseViewModel
    {
        public ElementDeviceViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        Rectangle deviceRectangle;
        Rectangle mouseOverRectangle;
        Rectangle selectationRectangle;
        public ElementDevice elementDevice;

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

        public void Initialize(ElementDevice elementDevice, Canvas canvas)
        {
            this.elementDevice = elementDevice;

            if (ServiceClient.CurrentConfiguration.AllDevices.Any(x => x.Path == elementDevice.Path))
            {
                device = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == elementDevice.Path);
                Firesec.Metadata.drvType Driver = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                Name = Driver.shortName;
            }

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, elementDevice.Left);
            Canvas.SetTop(innerCanvas, elementDevice.Top);

            innerCanvas.ToolTip = Name;

            canvas.Children.Add(innerCanvas);

            deviceRectangle = new Rectangle();
            deviceRectangle.Width = 20;
            deviceRectangle.Height = 20;
            deviceRectangle.Fill = Brushes.Blue;
            innerCanvas.Children.Add(deviceRectangle);

            Polyline polyline = new Polyline();
            polyline.Points.Add(new System.Windows.Point(11, 2));
            polyline.Points.Add(new System.Windows.Point(7, 11));
            polyline.Points.Add(new System.Windows.Point(13, 8));
            polyline.Points.Add(new System.Windows.Point(8, 18));
            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 1;
            polyline.StrokeLineJoin = PenLineJoin.Round;
            innerCanvas.Children.Add(polyline);

            mouseOverRectangle = new Rectangle();
            mouseOverRectangle.Width = 20;
            mouseOverRectangle.Height = 20;
            mouseOverRectangle.Stroke = Brushes.Red;
            innerCanvas.Children.Add(mouseOverRectangle);

            selectationRectangle = new Rectangle();
            selectationRectangle.Width = 20;
            selectationRectangle.Height = 20;
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
            //string path = @"F8340ECE-C950-498D-88CD-DCBABBC604F3:Компьютер/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:6/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.62";
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

        void OnDeviceStateChanged(string path)
        {
            if (path == elementDevice.Path)
            {
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
                StateType stateType = StateHelper.NameToType(deviceState.State);
                switch (stateType)
                {
                    case StateType.Alarm:
                        deviceRectangle.Fill = Brushes.Red;
                        break;

                    case StateType.Failure:
                        deviceRectangle.Fill = Brushes.Red;
                        break;

                    case StateType.Info:
                        deviceRectangle.Fill = Brushes.YellowGreen;
                        break;

                    case StateType.No:
                        deviceRectangle.Fill = Brushes.Transparent;
                        break;

                    case StateType.Norm:
                        deviceRectangle.Fill = Brushes.Green;
                        break;

                    case StateType.Off:
                        deviceRectangle.Fill = Brushes.Red;
                        break;

                    case StateType.Service:
                        deviceRectangle.Fill = Brushes.Yellow;
                        break;

                    case StateType.Unknown:
                        deviceRectangle.Fill = Brushes.Gray;
                        break;

                    case StateType.Warning:
                        deviceRectangle.Fill = Brushes.Yellow;
                        break;
                }
            }
        }
    }
}
