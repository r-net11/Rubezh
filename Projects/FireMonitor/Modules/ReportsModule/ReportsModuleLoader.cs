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

		public ReportsModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);
			ReportsViewModel = new ReportsViewModel();
		}

		void OnShowReports(object obj)
		{
			ServiceFactory.Layout.Show(ReportsViewModel);
		}

		public override void Initialize()
		{
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