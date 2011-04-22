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

namespace DeviceEditor
{
    /// <summary>
    /// Логика взаимодействия для StatesView.xaml
    /// </summary>
    public partial class StatesView : UserControl
    {
        public StatesView()
        {
            InitializeComponent();
        }

        private void ListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ListBox)sender).SelectedItem = ((ListBox)sender).SelectedItem;
        }
    }
}
