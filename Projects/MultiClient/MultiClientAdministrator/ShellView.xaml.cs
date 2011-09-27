using System.Windows;
using MultiClient.Services;
using MultiClient.ViewModels;

namespace MultiClient
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            var layoutService = new LayoutService();
            layoutService.Initialize(this);
        }

        public IViewPart MainContent
        {
            get { return _mainRegionHost.Content as IViewPart; }
            set { _mainRegionHost.DataContext = _mainRegionHost.Content = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var serverViewModel = new ServerViewModel();
            ServiceFactory.Layout.Show(serverViewModel);
        }
    }
}
