using System.Windows;
using System.Windows.Controls;

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

        private void ListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            sender = new ListBox();
        }

        private void ListBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((ListBox)sender).SelectedItem = ((ListBox)sender).SelectedItem;
        }

    }
}