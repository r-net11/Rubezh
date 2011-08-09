using System.Windows.Controls;
using LibraryModule.ViewModels;

namespace LibraryModule.Views
{
    public partial class StatesView : UserControl
    {
        public StatesView()
        {
            InitializeComponent();
        }

        void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var stateViewModel = checkBox.DataContext as StateViewModel;
            (DataContext as DeviceViewModel).SelectedStateViewModel = stateViewModel;
        }
    }
}