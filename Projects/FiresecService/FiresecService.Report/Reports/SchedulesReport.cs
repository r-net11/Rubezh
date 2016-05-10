using System;
using System.Collections.Generic;
using System.Data;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class SchedulesReport : BaseReport<List<SchedulesData>>
	{
		public override List<SchedulesData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<SchedulesReportFilter>(f);
			var result = new List<SchedulesData>();

			var employees = dataProvider.GetEmployees(filter);
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.ScheduleUID == Guid.Empty || !filter.ScheduleSchemas.Contains(employee.Item.ScheduleUID))
						continue;
				}

				var dataRow = new SchedulesData();
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

				result.Add(dataRow);
			}

			return result;
		}
	}
}
