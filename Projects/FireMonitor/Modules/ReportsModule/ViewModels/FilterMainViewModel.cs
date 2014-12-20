using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;

namespace ReportsModule.ViewModels
{
	public class FilterMainViewModel : FilterContainerViewModel
	{
		public FilterMainViewModel()
		{
			Title = "Настройки";
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
		}
	}
}
