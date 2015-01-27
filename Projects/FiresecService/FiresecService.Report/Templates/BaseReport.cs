using System;
using System.Linq;
using FiresecAPI.SKD.ReportFilters;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report.Templates
{
	public partial class BaseReport : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
	{
		private float _topMargin;
		private float _bottomMargin;
		public BaseReport()
		{
			InitializeComponent();
			_topMargin = TopMargin.HeightF;
			_bottomMargin = BottomMargin.HeightF;
			Name = ReportTitle;
		}

		private void BaseReport_DataSourceDemanded(object sender, EventArgs e)
		{
			if (DesignMode)
				return;
			DataSourceRequered();
		}
		protected virtual void DataSourceRequered()
		{
		}

		public virtual string ReportTitle
		{
			get { return ""; }
		}
		protected SKDReportFilter Filter { get; private set; }

		#region IFilteredReport Members

		public virtual void ApplyFilter(SKDReportFilter filter)
		{
			Filter = filter;
			TopMargin.HeightF = _topMargin;
			BottomMargin.HeightF = _bottomMargin;
			ReportName.Value = filter.PrintFilterNameInHeader ? string.Format("{0} ({1})", ReportTitle, filter.Name) : ReportTitle;
			FilterName.Value = filter.Name;
			Timestamp.Value = filter.Timestamp;
			UserName.Value = filter.User;
			var periodFilter = filter as IReportFilterPeriod;
			if (periodFilter != null)
				Period.Value = string.Format("c {0:dd.MM.yyyy HH:mm:ss} по {1:dd.MM.yyyy HH:mm:ss}", periodFilter.DateTimeFrom, periodFilter.DateTimeTo);
			lTimestamp.Visible = filter.PrintDate;
			lFilterName.Visible = filter.PrintFilterName;
			lPeriod.Visible = filter.PrintPeriod && periodFilter != null;
			lUserName.Visible = filter.PrintUser;
			if (!lFilterName.Visible)
				TopMargin.HeightF -= lFilterName.HeightF;
			if (!lPeriod.Visible)
				TopMargin.HeightF -= lPeriod.HeightF;
			if (!lTimestamp.Visible)
				BottomMargin.HeightF -= lTimestamp.HeightF;
			if (!lUserName.Visible && lTimestamp.Visible)
				BottomMargin.HeightF -= lUserName.HeightF;
			ReportPrintOptions.DetailCountOnEmptyDataSource = 0;
			ApplySort();
		}

		#endregion

		protected virtual void ApplySort()
		{
			if (string.IsNullOrEmpty(Filter.SortColumn))
				return;
			var details = GetDetailBand();
			if (details == null)
				return;
			details.SortFields.Add(new GroupField(Filter.SortColumn, Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
		}
		protected DetailBand GetDetailBand()
		{
			return Bands.OfType<DetailBand>().FirstOrDefault();
		}
	}
}
