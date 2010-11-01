using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ComServerEmulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ComServerEmulatorView window1 = new ComServerEmulatorView();
            ComServerEmulatorViewModel viewModel = new ComServerEmulatorViewModel();
            window1.DataContext = viewModel;
            window1.Show();
        }
    }
}
