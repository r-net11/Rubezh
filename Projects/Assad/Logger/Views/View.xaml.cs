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
using System.Windows.Shapes;
using System.Xml;

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
