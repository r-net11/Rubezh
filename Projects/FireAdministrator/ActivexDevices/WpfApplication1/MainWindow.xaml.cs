using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeviceControls;
using Common;
using Firesec;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<DeviceControl> deviceControls = new List<DeviceControl>();
            //DevicesControl.ViewModel.DeviceManagerLoad();
            DeviceControls.DeviceControl deviceControl = new DeviceControl();
            //deviceControl.viewModel.Name = "1";
            deviceControls.Add(deviceControl);
            DeviceControls.DeviceControl deviceControl2 = new DeviceControl();
            //deviceControl2.viewModel.Name = "2";
            deviceControls.Add(deviceControl2);

            DeviceControls.DeviceControl deviceControl3 = new DeviceControl();
            //deviceControl.viewModel.Name = "3";
            deviceControls.Add(deviceControl3);
            //DevicesControl.DeviceControl deviceControl4 = new DeviceControl();
            //deviceControl.viewModel.Name = "4";
            //DevicesControl.DeviceControl deviceControl5 = new DeviceControl();
            //DevicesControl.DeviceControl deviceControl6 = new DeviceControl();
            //DevicesControl.DeviceControl deviceControl7 = new DeviceControl();

            wrapPanel.Children.Add(deviceControl);
            wrapPanel.Children.Add(deviceControl2);
            wrapPanel.Children.Add(deviceControl3);
            //dockPanel.Children.Add(deviceControl4);
            //dockPanel.Children.Add(deviceControl5);
            //dockPanel.Children.Add(deviceControl6);
            //dockPanel.Children.Add(deviceControl7);

            deviceControl.DriverId = DriversHelper.GetDriverNameById("4935848F-0084-4151-A0C8-3A900E3CB5C5");
            deviceControl.StateId = "Неисправность";

            List<String> list1 = new List<string>();
            List<String> list2 = new List<string>();
            string str1 = "Ход на открытие";
            string str2 = "Открыто";
            string str3 = "Ход на закрытие";
            list1.Add(str1);
            list2.Add(str2);
            list2.Add(str3);
            deviceControl.AdditionalStates = list1;

            //return;

            deviceControl2.DriverId = DriversHelper.GetDriverNameById("4935848F-0084-4151-A0C8-3A900E3CB5C5");
            deviceControl2.StateId = "Неисправность";
            deviceControl2.AdditionalStates = list2;

            deviceControl3.DriverId = DriversHelper.GetDriverNameById("799686B6-9CFA-4848-A0E7-B33149AB940C");
            deviceControl3.StateId = "Тревога";

            //deviceControl4.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
            //deviceControl4.StateId = "Тревога";

            //deviceControl5.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
            //deviceControl5.StateId = "Норма";

            //deviceControl6.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
            //deviceControl6.StateId = "Норма";

            //deviceControl7.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
            //deviceControl7.StateId = "Норма";

        }

             
    }
}
