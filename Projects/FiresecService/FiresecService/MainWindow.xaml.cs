using System.Windows;
using FiresecService;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FiresecManager.Connect("adm", "");
            FiresecServiceManager.Open();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
