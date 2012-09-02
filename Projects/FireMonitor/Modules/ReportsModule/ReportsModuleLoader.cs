using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using ReportsModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Reports;
using Infrastructure.Client;

namespace ReportsModule
{
	public class ReportsModuleLoader : ModuleBase
	{
		private ReportsViewModel _reportViewModel;

		public ReportsModuleLoader()
		{
			_reportViewModel = new ReportsViewModel();
		}

		public override void Initialize()
		{
			foreach (var module in ApplicationService.Modules)
			{
				var reportProviderModule = module as IReportProviderModule;
				if (reportProviderModule != null)
					_reportViewModel.AddReports(reportProviderModule.GetReportProviders());
			}
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
		    {
		        new NavigationItem<ShowReportsEvent>(_reportViewModel, "Отчеты", "/Controls;component/Images/levels.png"),
		    };
		}

		public override string Name
		{
			get { return "Отчеты"; }
		}
	}
}