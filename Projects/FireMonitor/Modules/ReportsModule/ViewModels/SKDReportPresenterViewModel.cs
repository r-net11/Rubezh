using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using DevExpress.Xpf.Printing;
using Infrastructure.Common.Windows;
using FiresecAPI.Automation;
using FiresecAPI.Models;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		public SKDReportPresenterViewModel()
		{
			ServiceKnownTypeProvider.Register<Parameter>();
			Model = new XReportServicePreviewModel("http://127.0.0.1:2323/FiresecReportService/");
			Model.IsParametersPanelVisible = false;
			Model.AutoShowParametersPanel = false;
			Model.CreateDocumentError += Model_CreateDocumentError;
		}

		private void Model_CreateDocumentError(object sender, FaultEventArgs e)
		{
			e.Handled = true;
			MessageBoxService.ShowException(e.Fault);
		}

		private SKDReportBaseViewModel _selectedReport;
		public SKDReportBaseViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				if (value != SelectedReport)
				{
					_selectedReport = value;
					OnPropertyChanged(() => SelectedReport);
					var reportViewModel = SelectedReport as SKDReportViewModel;
					if (reportViewModel != null)
					{
						Model.ReportName = reportViewModel.ReportProvider.Name;
						var plan = new Parameter()
						{
							Name = "TestParam",
						};
						Model.Build(plan);
					}
				}
			}
		}

		private XReportServicePreviewModel _model;
		public XReportServicePreviewModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged(() => Model);
			}
		}
		
	}
	public class XReportServicePreviewModel : ReportServicePreviewModel
	{
		public XReportServicePreviewModel(string s)
			: base(s)
		{
		}
		public void Build(object args)
		{
			CreateDocument(args);
		}
	}
}
