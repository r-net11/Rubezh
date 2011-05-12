using System.Windows;
using System.Windows.Controls;

namespace DeviceEditor.Views
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
            //((ListBox) sender).SelectedItem = ((ListBox) sender).SelectedItem;
        }
    }
}