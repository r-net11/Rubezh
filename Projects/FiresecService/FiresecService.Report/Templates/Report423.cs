using System;
using FiresecAPI;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;

namespace FiresecService.Report.Templates
{
    public partial class Report423 : BaseReport
    {
        public Report423()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Отчет по оправдательным документам"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter423>();

            var employees = dataProvider.GetEmployees(filter);

            var ds = new DataSet423();
            foreach (var employee in employees)
            {
                var documentsResult = dataProvider.DatabaseService.TimeTrackDocumentTranslator.Get(employee.UID, filter.DateTimeFrom, filter.DateTimeTo);
                if (documentsResult.Result != null)
                {
                    foreach (var document in documentsResult.Result)
                    {
                        if (filter.Abcense && document.TimeTrackDocumentType.DocumentType == DocumentType.Absence ||
                           filter.Presence && document.TimeTrackDocumentType.DocumentType == DocumentType.Presence ||
                            filter.Overtime && document.TimeTrackDocumentType.DocumentType == DocumentType.Overtime)
                        {
                            var row = ds.Data.NewDataRow();
                            row.Employee = employee.Name;
                            row.Department = employee.Department;
                            row.StartDateTime = document.StartDateTime;
                            row.EndDateTime = document.EndDateTime;
                            row.DocumentCode = document.TimeTrackDocumentType.Code;
                            row.DocumentName = document.TimeTrackDocumentType.Name;
                            row.DocumentShortName = document.TimeTrackDocumentType.ShortName;
                            row.DocumentType = document.TimeTrackDocumentType.DocumentType.ToDescription();
                            ds.Data.AddDataRow(row);
                        }
                    }
                }
            }
            return ds;
        }
    }
}
