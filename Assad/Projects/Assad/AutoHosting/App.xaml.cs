using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AssadProcessor;

namespace AutoHosting
{
    public partial class App : Application
    {
        View view;
        ViewModel viewModel;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            view = new View();
            viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //viewModel.CloseCommand.Execute(null);
            //Controller.Current.Stop();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception occured");
            //var result = Logger.Logger.SaveLogToFile();
        }
    }
}
