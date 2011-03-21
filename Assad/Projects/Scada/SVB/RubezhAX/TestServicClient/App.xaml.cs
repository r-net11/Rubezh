using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;

namespace TestServiceClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static StreamWriter debugStream;

        //public static void DebugMessage(string mess)
        //{
        //    if (debugStream != null)
        //    {
        //        debugStream.WriteLine(DateTime.Now.ToString("dd-MM hh:mm:ss") + Current.ToString() + mess);
        //    }


        //}


        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            Window1 view = new Window1();
            ViewModel viewModel = new ViewModel();
            viewModel.form = view;
            if (viewModel.goMethodAPI() == false) Shutdown();
            view.DataContext = viewModel;
            view.Show();

        }
    }
}
