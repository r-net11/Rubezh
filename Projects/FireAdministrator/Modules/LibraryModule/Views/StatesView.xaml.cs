using System.Windows;
using System.Windows.Controls;


namespace LibraryModule.Views
{
    public partial class StatesView : UserControl
    {
        public StatesView()
        {
            InitializeComponent();
        }

        private void ListBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((ListBox)sender).SelectedItem = ((ListBox)sender).SelectedItem;
        }
    }
}
