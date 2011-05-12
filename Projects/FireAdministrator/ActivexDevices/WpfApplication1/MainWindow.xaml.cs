using System;
using System.Collections.Generic;
using System.Windows;
using DeviceControls;


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

            for (var i = 0; i < 1000; i++)
            {
                var deviceControl = new DeviceControl();
                deviceControl.Margin = new Thickness(1);
                deviceControl.Width = deviceControl.Height = 50;

                deviceControl.DriverId = "4935848F-0084-4151-A0C8-3A900E3CB5C5";
                deviceControl.StateId = "2";

                var stateList = new List<string>();
                stateList.Add("32");
                stateList.Add("36");
                stateList.Add("37");
                deviceControl.AdditionalStatesIds = stateList;

                wrapPanel.Children.Add(deviceControl);
            }

            for (var i = 0; i < 1000; i++)
            {
                var deviceControl = new DeviceControl();
                deviceControl.Margin = new Thickness(1);
                deviceControl.Width = deviceControl.Height = 50;
                deviceControl.DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C";
                deviceControl.StateId = "0";

                wrapPanel.Children.Add(deviceControl);
            }
        }
    }
}
