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

namespace SoundsModule.Views
{
    /// <summary>
    /// Логика взаимодействия для SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : Window
    {
        public SaveDialog()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
        }
    }
}
