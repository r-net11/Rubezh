using System.Windows;
using System.Windows.Controls;

namespace Logger
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : Window
    {
        public View()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogEntry logEntry = (sender as ListBox).SelectedItem as LogEntry;
            ViewModel viewModel = DataContext as ViewModel;
            viewModel.SelectedEntry = logEntry;

            xmlViewer.xmlDocument = viewModel.SelectedXml;
        }
    }
}