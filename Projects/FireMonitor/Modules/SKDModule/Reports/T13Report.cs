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
		bool _isLandscape;
		public ReportModel ReportModel { get; private set; }

		public T13Report(bool isLandscape, ReportModel reportModel)
		{
			_isLandscape = isLandscape;
			ReportModel = reportModel;
		}

		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			return new ReportData();
		}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return _isLandscape ? "Reports/T13.xaml" : "Reports/T13_2.xaml"; }
		}

		public string Title
		{
			get { return "T13"; }
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