using System;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecService.Report.Templates
{
    public partial class BaseReport : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
    {
        public BaseReport()
        {
            InitializeComponent();
        }

        private void BaseReport_DataSourceDemanded(object sender, EventArgs e)
        {
            DataSourceRequered();
        }
        protected virtual void DataSourceRequered()
        {
        }

        public virtual string ReportTitle
        {
            get { return ""; }
        }

        #region IFilteredReport Members

        public virtual void ApplyFilter(SKDReportFilter filter)
        {
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
        }

        #endregion
    }
}
