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
using System.Xml.Serialization;
using System.IO;

namespace DeviveModelManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class View : Window
    {
        public View()
        {
            InitializeComponent();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeItem device = e.NewValue as TreeItem;
            (DataContext as ViewModel).SelectedDevice = device;
        }
    }
}
