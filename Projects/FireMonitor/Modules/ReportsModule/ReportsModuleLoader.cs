using System.Collections.Generic;
using FiresecAPI;
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
				new NavigationItem<ShowSKDReportsEvent>(_skdReportViewModel, "Отчеты 2", "/Controls;component/Images/levels.png"),
				new NavigationItem<ShowReportsEvent>(_reportViewModel, ModuleType.ToDescription(), "/Controls;component/Images/levels.png"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Reports; }
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Reports, "Отчеты", "Levels.png", (p) => _reportViewModel);
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
			yield return new TESTREPORT();
			yield return new SKDReportProvider("Report", "Test1", 10, null);
			yield return new SKDReportProvider("XtraReport1", "Test2", 110, null);
			yield return new SKDReportProvider("Report", "Test3", 211, null);
			yield return new SKDReportProvider("XtraReport1", "Test4", 210, null);
			yield return new SKDReportProvider("Report", "Test5", 50, SKDReportGroup.HR);
			yield return new SKDReportProvider("XtraReport1", "Test6", 10, SKDReportGroup.HR);
			yield return new SKDReportProvider("Report", "Test7", 60, SKDReportGroup.HR);
			yield return new SKDReportProvider("XtraReport1", "Test8", 10, SKDReportGroup.WorkingTime);
			yield return new SKDReportProvider("Report", "Test9", 20, SKDReportGroup.WorkingTime);
		}

		#endregion
	}
}