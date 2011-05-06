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

            for (int i = 0; i < 5; i++)
            {
                DeviceControls.DeviceControl deviceControl = new DeviceControl();
                deviceControl.Width = deviceControl.Height = 50;

                deviceControl.DriverId = "4935848F-0084-4151-A0C8-3A900E3CB5C5";
                deviceControl.StateId = "2";

                List<String> stateList = new List<string>();
                stateList.Add("32");
                stateList.Add("36");
                stateList.Add("37");
                deviceControl.AdditionalStatesIds = stateList;

                wrapPanel.Children.Add(deviceControl);
            }
        }
    }
}
