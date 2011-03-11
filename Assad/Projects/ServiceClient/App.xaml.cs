﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ServiceClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            View view = new View();
            ViewModel viewModel = new ViewModel();
            Controller controller = new Controller();
            controller.viewModel = viewModel;
            viewModel.controller = controller;
            view.DataContext = viewModel;
            view.Show();
        }
    }
}
