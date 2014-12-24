using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI;

namespace FiresecService.Report.Templates
{
    public class BaseSKDReport<T, TItem, TDataSet> : BaseReport
        where T : SKDReportFilter
    {
        protected T Filter { get; set; }

        public override void ApplyFilter(SKDReportFilter filter)
        {
            base.ApplyFilter(filter);
            Filter = (T)filter;
        }
        protected override void DataSourceRequered()
        {
            //using (var service = new SKDDatabaseService())
            //{
            //    var result = GetData(service);
            //    if (result != null && !result.HasError)
            //        DataSource = result.Result.ToDataSet<TDataSet>();
            //}
        }
        protected OperationResult<IEnumerable<TItem>> GetData(SKDDatabaseService service)
        {
            return null;
        }
    }
}
