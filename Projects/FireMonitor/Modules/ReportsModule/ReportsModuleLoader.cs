using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Enums;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Events.Reports;
using ReportsModule.ViewModels;
using Infrastructure.Common.SKDReports;
using System.Linq;

namespace ReportsModule
{
	public class ReportsModuleLoader : ModuleBase, ILayoutProviderModule, ISKDReportProviderModule
	{
		private ReportsViewModel _reportViewModel;
		private SKDReportsViewModel _skdReportViewModel;

		public override void CreateViewModels()
		{
			_reportViewModel = new ReportsViewModel();
			_skdReportViewModel = new SKDReportsViewModel();

			ServiceFactory.Events.GetEvent<PrintReportEvent>().Unsubscribe(OnPrintReport);
			ServiceFactory.Events.GetEvent<PrintReportEvent>().Subscribe(OnPrintReport);
			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Unsubscribe(OnPrintReportPreview);
			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Subscribe(OnPrintReportPreview);
		}

		public override void Initialize()
		{
			_reportViewModel.Reports.Clear();
			foreach (var module in ApplicationService.Modules)
			{
				var reportProviderModule = module as IReportProviderModule;
				if (reportProviderModule != null)
					_reportViewModel.AddReports(reportProviderModule.GetReportProviders());
				var skdReportProviderModule = module as ISKDReportProviderModule;
				if (skdReportProviderModule != null)
					_skdReportViewModel.RegisterReportProviderModule(skdReportProviderModule);
			}
			_skdReportViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSKDReportsEvent>(_skdReportViewModel, ModuleType.ToDescription(), "levels"),
				//new NavigationItem<ShowReportsEvent>(_reportViewModel, ModuleType.ToDescription(), "levels"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Reports; }
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Reports, "Отчеты", "Levels.png", (p) => _skdReportViewModel);
			//yield return new LayoutPartPresenter(LayoutPartIdentities.Reports, "Отчеты", "Levels.png", (p) => _reportViewModel);
		}

		#endregion

		private void OnPrintReport(IReportProvider provider)
		{
			var reportViewModel = new ReportPreviewViewModel(provider);
			reportViewModel.Print();
		}
		private void OnPrintReportPreview(IReportProvider provider)
		{
			DialogService.ShowModalWindow(new ReportPreviewViewModel(provider));
		}

		#region ISKDReportProviderModule Members

		public IEnumerable<ISKDReportProvider> GetSKDReportProviders()
		{
			return Enumerable.Empty<ISKDReportProvider>();
		}

		#endregion
	}
}