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
using System.Windows.Shapes;
using FiresecWcfService.Service;

namespace FiresecWcfService
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            FiresecServiceManager.Open();
            statusTextBlock.Text = "Running";
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            FiresecServiceManager.Close();
            statusTextBlock.Text = "Stopped";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FiresecServiceManager.Close();
        }

        ~MainWindow()
        {
            FiresecServiceManager.Close();
        }
    }
}
