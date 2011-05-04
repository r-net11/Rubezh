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

            for (int i = 0; i < 10; i++)
            {
                DeviceControls.DeviceControl deviceControl = new DeviceControl();
                deviceControl.Width = deviceControl.Height = 50;

                deviceControl.DriverId = "4935848F-0084-4151-A0C8-3A900E3CB5C5";
                deviceControl.StateId = "2";

                List<String> stateList = new List<string>();
                stateList.Add("32");
                stateList.Add("36");
                //deviceControl.AdditionalStates = stateList;

                wrapPanel.Children.Add(deviceControl);
            }

            for (int i = 0; i < 10; i++)
            {
                DeviceControls.DeviceControl deviceControl = new DeviceControl();
                deviceControl.Width = deviceControl.Height = 50;

                deviceControl.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
                deviceControl.StateId = "0";

                wrapPanel.Children.Add(deviceControl);
            }
        }
    }
}
