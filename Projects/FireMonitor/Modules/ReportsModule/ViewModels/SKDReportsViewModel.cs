using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure;
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
			Enum.GetValues(typeof(SKDReportGroup)).Cast<SKDReportGroup>().ForEach(group =>
			{
				// Не отображать элемент главного меню "Отчеты / Учет рабочего времени", если этого требует лицензия
				//if (group == SKDReportGroup.TimeTracking &&
				//	!ServiceFactory.UiElementsVisibilityService.IsMainMenuReportsUrvElementVisible)
				//	return;
				
				Reports.Add(new SKDReportGroupViewModel(group));
			});
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
			module.GetSKDReportProviders().Where(CheckPermission).ForEach(RegisterReportProvider);
		}
		private bool CheckPermission(ISKDReportProvider provider)
		{
			return !provider.Permission.HasValue || FiresecManager.CheckPermission(provider.Permission.Value);
		}
		private void RegisterReportProvider(ISKDReportProvider provider)
		{
			if (provider.Group.HasValue)
			{
				// Не регистрируем группу отчетов УРВ, если этого требует лицензия
				//if (provider.Group.Value == SKDReportGroup.TimeTracking &&
				//	!ServiceFactory.UiElementsVisibilityService.IsMainMenuSkdUrvElementVisible)
				//	return;

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
	}
}
