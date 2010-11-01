using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            MainWindowView view = new MainWindowView();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            view.DataContext = viewModel;
            viewModel.mainWindowView = view;
            view.Show();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception occured");
            var result = Logger.Logger.SaveLogToFile();
        }
    }
}
