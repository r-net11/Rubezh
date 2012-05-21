using System;
using System.ComponentModel;
using System.Windows;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using ReportsModule.ViewModels;
using ReportsModule.Views;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace ReportsModule
{
	public class ReportsModuleLoader : ModuleBase
	{
		ReportsViewModel ReportsViewModel;
		ReportsWindow reportsWindow;

		public ReportsModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);
		}

		void CreateViewModels()
		{
			ReportsViewModel = new ReportsViewModel();
		}

		void OnShowReports(object obj)
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

		public void PreLoad()
		{
			return;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
			backgroundWorker.RunWorkerAsync();
		}

		void InitializeReportsWindow()
		{
			reportsWindow = new ReportsWindow();
			reportsWindow.Width = 0;
			reportsWindow.Height = 0;
			reportsWindow.Show();
			reportsWindow.Initialize();
			reportsWindow.Hide();
		}
		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ServiceFactory.ShellView.Dispatcher.BeginInvoke(new Action(() =>
			{
				InitializeReportsWindow();
			}));
		}

		public override void Initialize()
		{
			CreateViewModels();
			PreLoad();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowReportsEvent>("Отчеты", "/Controls;component/Images/levels.png"),
			};
		}
	}
}