using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResursIncotexTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void EventhHandler_MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Application.Current.Shutdown();
        }

        #endregion

        private void EventhHandler_CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show("Команда запущена из " + e.Source.ToString(), 
                "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
