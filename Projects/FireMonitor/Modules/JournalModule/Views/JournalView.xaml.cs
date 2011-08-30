using System.Windows.Controls;
using System.Windows;

namespace JournalModule.Views
{
    public partial class JournalView : UserControl
    {
        public JournalView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DevicesView_Loaded);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (journalDataGrid.SelectedItem != null)
            {
                journalDataGrid.ScrollIntoView(journalDataGrid.SelectedItem);
            }
        }
    }
}