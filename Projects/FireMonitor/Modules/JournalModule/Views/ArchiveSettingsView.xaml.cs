using System.Windows.Controls;
using JournalModule.ViewModels;

namespace JournalModule.Views
{
    public partial class ArchiveSettingsView : UserControl
    {
        public ArchiveSettingsView()
        {
            InitializeComponent();
        }

        void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkedArchiveDefaultStateViewModel = (sender as RadioButton).DataContext as ArchiveDefaultStateViewModel;
            foreach (ArchiveDefaultStateViewModel radioButton in _archiveDefaultStateTypes.Items)
            {
                if (radioButton != checkedArchiveDefaultStateViewModel)
                    radioButton.IsActive = false;
            }
        }
    }
}