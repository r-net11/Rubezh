using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class PositionPageViewModel : FilterContainerViewModel
	{
		public PositionPageViewModel()
		{
			Title = "Должность";
			Filter = new PositionsFilterViewModel();
		}

		public PositionsFilterViewModel Filter { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var positionFilter = filter as IReportFilterPosition;
			Filter.Initialize(positionFilter == null ? null : positionFilter.Positions, FiresecAPI.SKD.LogicalDeletationType.Active);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var positionFilter = filter as IReportFilterPosition;
			if (positionFilter != null)
				positionFilter.Positions = Filter.UIDs;
		}
	}
}
