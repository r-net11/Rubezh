using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using ReportsModule.ViewModels;
using FiresecAPI.Models;
using System.ComponentModel;
using System;
using System.Windows;
using ReportsModule.Views;

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
            if (false)
            {
                reportsWindow.WindowStyle = WindowStyle.ThreeDBorderWindow;
                reportsWindow.Width = 500;
                reportsWindow.Height = 500;
                reportsWindow.Show();
            }
			
			ServiceFactory.Layout.Show(ReportsViewModel);
		}

		static ReportsWindow reportsWindow;

		public static void PreLoad()
		{
            return;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
			backgroundWorker.RunWorkerAsync();
		}

		static void InitializeReportsWindow()
		{
			reportsWindow = new ReportsWindow();
			reportsWindow.Width = 0;
			reportsWindow.Height = 0;
			reportsWindow.Show();
			reportsWindow.Initialize();
			reportsWindow.Hide();
		}

		static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ServiceFactory.ShellView.Dispatcher.BeginInvoke(new Action(() =>
			{
				InitializeReportsWindow();
			}));
		}
	}
}