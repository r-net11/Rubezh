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
using System.Diagnostics;

namespace DevicesModule.Views
{
    /// <summary>
    /// Логика взаимодействия для DevicesView.xaml
    /// </summary>
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
            Current = this;
            this.Loaded += new RoutedEventHandler(DevicesView_Loaded);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollToSelected();
        }

        void ScrollToSelected()
        {
            if (Current.dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }

        public static DevicesView Current;
    }
}
