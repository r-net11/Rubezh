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
using System.ServiceModel;
using ServiceApi;
using ClientApi;

namespace ServiceClient2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        ServiceClient serviceClient;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            serviceClient = new ServiceClient();
            serviceClient.Start();
            serviceClient.DeviceStateChanged += new Action<Device>(serviceClient_DeviceStateChanged);
        }

        void serviceClient_DeviceStateChanged(Device obj)
        {
            MessageBox.Show("Hello");
        }
    }
}
