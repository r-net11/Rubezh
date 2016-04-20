using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhClient;
using Infrastructure.Common.Windows.SKDReports;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace ReportsModule.ViewModels
{
	public class SKDReportsViewModel : ViewPartViewModel
	{
		public SKDReportsViewModel()
		{
			ReportPresenter = new SKDReportPresenterViewModel();
			Reports = new ObservableCollection<SKDReportBaseViewModel>();
			Enum.GetValues(typeof(SKDReportGroup)).Cast<SKDReportGroup>().ForEach(group => Reports.Add(new SKDReportGroupViewModel(group)));
		}

		public void Initialize()
		{
			ReportPresenter.CreateClient();
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

			var list = module.GetSKDReportProviders();
			module.GetSKDReportProviders().Where(CheckPermission).ForEach(RegisterReportProvider);
			if (!IsCheckedPermission)
			{
				IsCheckedPermission = module.GetSKDReportProviders().Any(CheckPermission);
			}
		}
		private bool CheckPermission(ISKDReportProvider provider)
		{
			return !provider.Permission.HasValue || ClientManager.CheckPermission(provider.Permission.Value);
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

		public override void OnShow()
		{
			if (SelectedReport == null)
				SelectedReport = Reports.FirstOrDefault(item => item is SKDReportViewModel);
		}

		public bool IsCheckedPermission { get; private set; }
	}
}