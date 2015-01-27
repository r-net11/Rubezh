using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI;

namespace FiresecService.Report.Templates
{
    public partial class Report417 : BaseReport
    {
        public Report417()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Местонахождение сотрудников/посетителей"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter417>();

            var employees = dataProvider.GetEmployees(filter);
            var dataSet = new DataSet417();
            foreach (var employee in employees)
            {
                var dataRow = dataSet.Data.NewDataRow();

                dataRow.Employee = employee.Name;
                dataRow.Orgnisation = employee.Organisation;
                dataRow.Department = employee.Department;
                dataRow.Position = employee.Position;

                var passJournal = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeeLastPassJournal(employee.UID);
                if (passJournal != null)
                {
                    dataRow.EnterDateTime = passJournal.EnterTime;
                    if (passJournal.ExitTime.HasValue)
                    {
                        dataRow.ExitDateTime = passJournal.ExitTime.Value;
                        dataRow.Period = passJournal.ExitTime.Value - passJournal.EnterTime;
                    }
                    var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == passJournal.ZoneUID);
                    if (zone != null)
                    {
                        dataRow.Zone = zone.PresentationName;
                    }
                }

                dataSet.Data.Rows.Add(dataRow);
            }
            return dataSet;
        }
    }
}