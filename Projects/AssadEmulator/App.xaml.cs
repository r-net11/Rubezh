using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;

namespace AssadEmulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AssadEmulatorView view = new AssadEmulatorView();
            AssadEmulatorViewModel viewModel = new AssadEmulatorViewModel();
            view.DataContext = viewModel;
            view.Show();
        }

        public static void Create()
        {
            AssadEmulatorView view = new AssadEmulatorView();
            AssadEmulatorViewModel viewModel = new AssadEmulatorViewModel();
            view.DataContext = viewModel;
            ElementHost.EnableModelessKeyboardInterop(view);
            view.Show();
        }
    }
}
