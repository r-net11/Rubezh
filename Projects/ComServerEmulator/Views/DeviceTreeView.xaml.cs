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
using ComDevices;

namespace ComServerEmulator
{
    /// <summary>
    /// Interaction logic for DeviceTreeView.xaml
    /// </summary>
    public partial class DeviceTreeView : UserControl
    {
        public DeviceTreeView()
        {
            InitializeComponent();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ComDevice device = e.NewValue as ComDevice;
            (DataContext as ComServerEmulatorViewModel).SelectedDevice = device;
        }
    }
}
