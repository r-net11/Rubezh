using System;
using System.Data;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class SchedulesReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<SchedulesReportFilter>(f);
			var dataSet = new SchedulesDataSet();

			var employees = dataProvider.GetEmployees(filter);
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.ScheduleUID == Guid.Empty || !filter.ScheduleSchemas.Contains(employee.Item.ScheduleUID))
						continue;
				}

				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Employee = employee.Name;
				dataRow.Organisation = employee.Organisation;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;

				if (employee.Item.ScheduleUID != Guid.Empty)
				{
					dataRow.Schedule = employee.Item.ScheduleName;
					var scheduleResult = dataProvider.DbService.ScheduleTranslator.GetSingle(employee.Item.ScheduleUID);
					if (scheduleResult.Result != null)
					{
						dataRow.UseHoliday = !scheduleResult.Result.IsIgnoreHoliday;
						dataRow.FirstEnterLastExit = scheduleResult.Result.IsOnlyFirstEnter;
						dataRow.Delay = scheduleResult.Result.AllowedLate;
						dataRow.LeaveBefore = scheduleResult.Result.AllowedLate;

						var scheduleSchemeResult = dataProvider.DbService.ScheduleSchemeTranslator.GetSingle(scheduleResult.Result.ScheduleSchemeUID);
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
