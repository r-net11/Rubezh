using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TestServiceClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            Window1 view = new Window1();
            ViewModel viewModel = new ViewModel();
            ControlService controller = new ControlService();
            controller.form = view;
            viewModel.form = view;
            controller.viewModel = viewModel;
            viewModel.controller = controller;
            if (viewModel.goMethod() == false) Shutdown();
            view.DataContext = viewModel;
            view.Show();

        }
    }
}
