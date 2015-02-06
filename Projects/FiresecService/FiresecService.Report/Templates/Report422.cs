using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report422 : BaseReport
	{
		public Report422()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Отчет по графикам работы"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<ReportFilter422>();
			var dataSet = new DataSet422();

            var employees = dataProvider.GetEmployees(filter);
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.Schedule == null || !filter.ScheduleSchemas.Contains(employee.Item.Schedule.UID))
							continue;
				}

				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Employee = employee.Name;
                dataRow.Organisation = employee.Organisation;
                dataRow.Department = employee.Department;
                dataRow.Position = employee.Position;

				if (employee.Item.Schedule != null)
				{
                    dataRow.Schedule = employee.Item.Schedule.Name;
                    var scheduleResult = dataProvider.DatabaseService.ScheduleTranslator.GetSingle(employee.Item.Schedule.UID);
					if (scheduleResult.Result != null)
					{
						dataRow.UseHoliday = !scheduleResult.Result.IsIgnoreHoliday;
						dataRow.FirstEnterLastExit = scheduleResult.Result.IsOnlyFirstEnter;
						dataRow.Delay = scheduleResult.Result.AllowedLate;
						dataRow.LeaveBefore = scheduleResult.Result.AllowedLate;

						var scheduleSchemeResult = dataProvider.DatabaseService.ScheduleSchemeTranslator.GetSingle(scheduleResult.Result.ScheduleSchemeUID);
						if (scheduleSchemeResult.Result != null)
						{
							dataRow.BaseSchedule = scheduleSchemeResult.Result.Name;
							dataRow.ScheduleType = scheduleSchemeResult.Result.Type.ToDescription();
						}
					}
				}

				dataSet.Data.Rows.Add(dataRow);
			}

			return dataSet;
		}
	}
}