using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using ReportsModule.ViewModels;

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
			_reportViewModel.Reports.Clear();
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