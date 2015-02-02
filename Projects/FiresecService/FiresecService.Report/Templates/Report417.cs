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
            var journal = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesLastPassJournal(employees.Select(item => item.UID).ToList(), filter.Zones, filter.UseCurrentDate ? (DateTime?)null : filter.ReportDateTime).ToDictionary(item => item.EmployeeUID);
            var zoneMap = SKDManager.Zones.ToDictionary(item => item.UID);
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Employee = employee.Name;
				dataRow.Orgnisation = employee.Organisation;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;
                if (journal.ContainsKey(employee.UID))
                {
                    var record = journal[employee.UID];
                    dataRow.EnterDateTime = record.EnterTime;
                    if (zoneMap.ContainsKey(record.ZoneUID))
                        dataRow.Zone = zoneMap[record.ZoneUID].PresentationName;
                    if (filter.UseCurrentDate)
                        dataRow.Period = DateTime.Now - dataRow.EnterDateTime;
                    else
                    {
                        dataRow.Period = (record.ExitTime.HasValue ? record.ExitTime.Value : filter.ReportDateTime) - record.EnterTime;
                        if (record.ExitTime.HasValue)
                            dataRow.ExitDateTime = record.ExitTime.Value;
                    }
                }
				dataSet.Data.Rows.Add(dataRow);
			}
			return dataSet;
		}
	}
}