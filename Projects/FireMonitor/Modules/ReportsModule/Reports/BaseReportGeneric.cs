using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using FiresecAPI.Models;
using FiresecClient;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        public BaseReportGeneric()
        {
            DataList = new List<T>();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

		public override void LoadCrystalReportDocument(ReportDocument reportDocument)
        {
            //reportDocument.Load(FileHelper.GetReportFilePath(ReportFileName));
			reportDocument.FileName = FileHelper.GetReportFilePath(ReportFileName);
            reportDocument.SetDataSource(DataList);
        }
    }
}