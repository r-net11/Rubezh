using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using DevExpress.Xpf.Printing;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		public SKDReportPresenterViewModel()
		{
			Model = new ReportServicePreviewModel("http://127.0.0.1:2323/FiresecReportService/");
			Model.ReportName = "FiresecService.Report.Report, FiresecService.Report";
			Model.CreateDocument();
		}

		private SKDReportBaseViewModel _selectedReport;
		public SKDReportBaseViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				_selectedReport = value;
				OnPropertyChanged(() => SelectedReport);
				var reportViewModel = SelectedReport as SKDReportViewModel;
				if (reportViewModel != null)
				{
					//Model.ReportName = reportViewModel.ReportProvider.Name;
					//Model.CreateDocument();
				}
			}
		}

		private ReportServicePreviewModel _model;
		public ReportServicePreviewModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged(() => Model);
			}
		}
		
	}
}
