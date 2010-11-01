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

namespace Main
{
    /// <summary>
    /// Interaction logic for ComServerTreeView.xaml
    /// </summary>
    public partial class ComServerTreeView : UserControl
    {
        public ComServerTreeView()
        {
            InitializeComponent();
        }

        private void ComServerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Common.ComDevice device = e.NewValue as Common.ComDevice;
            (DataContext as MainWindowViewModel).SelectedComDevice = device;
        }
    }
}
