using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Runner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainViewModel mainViewModel = new MainViewModel();
            MainView mainView = new MainView();
            mainViewModel.MainView = mainView;
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
