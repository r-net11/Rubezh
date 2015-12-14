using FiresecAPI;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using System.Data;

namespace FiresecService.Report.Templates
{
	public partial class SchedulesReport : BaseReport
	{
		public SchedulesReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return "Отчет по графикам работы"; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<SchedulesReportFilter>();
			var dataSet = new SchedulesDataSet();

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
						dataRow.LeaveBefore = scheduleResult.Result.AllowedEarlyLeave;
						//TODO: Fill allowed absence and overtime

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