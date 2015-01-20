using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using DevExpress.Xpf.Printing;

namespace ReportsModule.ViewModels
{
	public class SKDReportViewModel : SKDReportBaseViewModel
	{
		private const string DefaultIconSource = "Week";

		public SKDReportViewModel(ISKDReportProvider reportProvider)
		{
			ReportProvider = reportProvider;
			HasGroupChildren = true;
			if (ReportProvider is IFilteredSKDReportProvider)
				ServiceKnownTypeProvider.Register(((IFilteredSKDReportProvider)ReportProvider).FilterType);
		}

		public ISKDReportProvider ReportProvider { get; private set; }
		public override string Title
		{
			get { return ReportProvider.Title; }
		}
		public override string IconSource
		{
			get { return ReportProvider.IconSource ?? DefaultIconSource; }
		}
		public override int SortIndex
		{
			get { return ReportProvider.Index; }
		}
	}
}
