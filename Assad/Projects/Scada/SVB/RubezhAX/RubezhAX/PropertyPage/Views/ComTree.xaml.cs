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
using RubezhAX.ServiceReference;
using RubezhAX;
//using ServiceClient;

namespace RubezhAX.PropertyPage.Views
{
    /// <summary>
    /// Interaction logic for ComTree.xaml
    /// </summary>
    public partial class ComTree : UserControl
    {
        public ComTree()
        {
            InitializeComponent();
        }

        private void ComServerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DeviceDescriptor device = e.NewValue as DeviceDescriptor;
            (DataContext as ViewModel).SelectedDevice = device;
        }
    }
}
