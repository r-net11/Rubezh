using CodeReason.Reports;
using FiresecAPI.SKD;
using Infrastructure.Common.Reports;

namespace SKDModule.Reports
{
	internal class T13Report : ISingleReportProvider
	{
		public ReportT13 ReportModel { get; private set; }

		public T13Report(ReportT13 reportModel)
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

			data.ReportDocumentValues.Add("FillName", ReportModel.FillName);
			data.ReportDocumentValues.Add("HRName", ReportModel.HRName);
			data.ReportDocumentValues.Add("LeadName", ReportModel.LeadName);

			data.ReportDocumentValues.Add("Organization", ReportModel.OrganizationName);
			data.ReportDocumentValues.Add("Department", ReportModel.DepartmentName);

			data.ReportDocumentValues.Add("DocNumber", ReportModel.DocNumber);
			data.ReportDocumentValues.Add("OKPO", ReportModel.OKPO);

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