using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using FiresecWcfService.Views;
using FiresecWcfService.ViewModel;

namespace FiresecWcfService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainView mainView = new MainView();
            MainViewModel mainViewModel = new MainViewModel();
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
