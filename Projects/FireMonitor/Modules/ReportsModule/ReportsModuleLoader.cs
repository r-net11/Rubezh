using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using ReportsModule.ViewModels;
using FiresecAPI.Models;
using System.ComponentModel;
using System;

namespace ReportsModule
{
    public class ReportsModuleLoader
    {
        static ReportsViewModel ReportsViewModel;

        public ReportsModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void Initialize()
        {
            //ReportsViewModel.Initialize();
        }

        static void CreateViewModels()
        {
            ReportsViewModel = new ReportsViewModel();
        }

        static void OnShowReports(object obj)
        {
            ServiceFactory.Layout.Show(ReportsViewModel);
        }

        public static void PreLoad()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerAsync();
        }

        static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ServiceFactory.ShellView.Dispatcher.BeginInvoke(new Action(() =>
            {
                ServiceFactory.Layout.PreLoad(ReportsViewModel);
                ReportsViewModel.SelectedReportName = ReportType.ReportDevicesList;
            }));
        }
    }
}