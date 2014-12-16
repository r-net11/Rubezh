using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Reports;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows.ViewModels;

namespace ReportsModule.ViewModels
{
	public class SKDReportsViewModel : ViewPartViewModel
	{
		public SKDReportsViewModel()
		{
			ReportPresenter = new SKDReportPresenterViewModel();
			Reports = new ObservableCollection<SKDReportBaseViewModel>();
			Enum.GetValues(typeof(SKDReportGroup)).Cast<SKDReportGroup>().ForEach(group => Reports.Add(new SKDReportGroupViewModel(group)));
			ServiceFactory.Events.GetEvent<NewReportProviderEvent>().Unsubscribe(OnNewReportProvider);
			ServiceFactory.Events.GetEvent<NewReportProviderEvent>().Subscribe(OnNewReportProvider);
		}

		public ObservableCollection<SKDReportBaseViewModel> Reports { get; private set; }
		public SKDReportPresenterViewModel ReportPresenter { get; private set; }

		private SKDReportBaseViewModel _selectedReport;
		public SKDReportBaseViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				_selectedReport = value;
				OnPropertyChanged(() => SelectedReport);
				ReportPresenter.SelectedReport = SelectedReport;
			}
		}

		public void RegisterReportProviderModule(ISKDReportProviderModule module)
		{
			module.GetSKDReportProviders().Where(item => CheckPermission(item)).ForEach(item => RegisterReportProvider(item));
		}
		private void RegisterReportProvider(ISKDReportProvider provider)
		{
			if (provider.Group.HasValue)
			{
				var groupViewModel = Reports.OfType<SKDReportGroupViewModel>().First(item => item.SKDReportGroup == provider.Group.Value);
				groupViewModel.AddChild(new SKDReportViewModel(provider));
			}
			else
				Reports.Add(new SKDReportViewModel(provider));
		}

		public void Initialize()
		{
			ReportPresenter.CreateClient();
		}

		public override void OnShow()
		{
			if (SelectedReport == null)
				SelectedReport = Reports.FirstOrDefault(item => item is SKDReportViewModel);
		}

		void OnNewReportProvider(ISKDReportProvider provider)
		{
			if(CheckPermission(provider))
				RegisterReportProvider(provider);
		}

		bool CheckPermission(ISKDReportProvider provider)
		{
			return !provider.Permission.HasValue || FiresecManager.CheckPermission(provider.Permission.Value);
		}
	}

	
}
