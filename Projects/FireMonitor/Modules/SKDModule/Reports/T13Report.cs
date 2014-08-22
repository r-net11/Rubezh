using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Reports;
using CodeReason.Reports;
using FiresecAPI.SKD;

namespace SKDModule.Reports
{
	internal class T13Report : ISingleReportProvider
	{
		public ReportModel ReportModel { get; private set; }

		public T13Report(ReportModel reportModel)
		{
			ReportModel = reportModel;
		}

		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", ReportModel.CreationDateTime);
			data.ReportDocumentValues.Add("StartDate", ReportModel.StartDateTime);
			data.ReportDocumentValues.Add("EndDate", ReportModel.EndDateTime);
			return data;
		}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/T13.xaml"; }
		}

		public string Title
		{
			get { return string.Format("Унифицированная форма №T-13 за период с {0:dd.MM.yyyy} по {1:dd.MM.yyyy}", ReportModel.StartDateTime, ReportModel.EndDateTime); }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider
		{
			get { return null; }
		}

		#endregion
	}
}