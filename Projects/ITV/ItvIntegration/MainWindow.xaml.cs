using System.Windows;

namespace ItvIntegration
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();
            DataContext = devicesViewModel;
        }
    }
}
