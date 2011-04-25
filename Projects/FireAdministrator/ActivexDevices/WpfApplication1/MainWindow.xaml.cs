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
using DevicesControl;
using Common;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DevicesControl.DeviceControl deviceControl = new DeviceControl();
            _content.Content = deviceControl;
            deviceControl.DriverId = "4935848F-0084-4151-A0C8-3A900E3CB5C5";

            deviceControl.StateId = "Норма";
            List<String> list = new List<string>();
            string str1 = "Открыто";
            string str2 = "Ход на закрытие";
            list.Add(str1);
            list.Add(str2);
            deviceControl.AdditionalStates = list;
            
        }

             
    }
}
