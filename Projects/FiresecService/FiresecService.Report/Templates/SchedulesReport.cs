﻿using System.Diagnostics;
using System.Linq;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD.ReportFilters;
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
			get { return CommonResources.WorkScheduleReport; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<SchedulesReportFilter>();
			var dataSet = new SchedulesDataSet();

			var employees = dataProvider.GetEmployees(filter).Where(x => x.Item.Schedule != null);
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

				Debug.Assert(employee.Item.Schedule != null, "employee.Item.Schedule != null");
				dataRow.Schedule = employee.Item.Schedule.Name;

				var scheduleResult = dataProvider.DatabaseService.ScheduleTranslator.GetSingle(employee.Item.Schedule.UID).Result;
				if (scheduleResult != null)
				{
					dataRow.UseHoliday = !scheduleResult.IsIgnoreHoliday;
					dataRow.FirstEnterLastExit = scheduleResult.IsOnlyFirstEnter;
					dataRow.AllowedLate = scheduleResult.AllowedLate == default(int) ? string.Empty : scheduleResult.AllowedLate.ToString();
					dataRow.AllowedEarlyLeave = scheduleResult.AllowedEarlyLeave == default(int) ? string.Empty : scheduleResult.AllowedEarlyLeave.ToString();
					dataRow.AllowedAbsence = scheduleResult.AllowedAbsentLowThan == default(int) ? string.Empty : scheduleResult.AllowedAbsentLowThan.ToString();
					dataRow.AllowedOvertime =  scheduleResult.NotAllowOvertimeLowerThan == default(int) ? string.Empty : scheduleResult.NotAllowOvertimeLowerThan.ToString();

					var scheduleSchemeResult = dataProvider.DatabaseService.ScheduleSchemeTranslator.GetSingle(scheduleResult.ScheduleSchemeUID);
					if (scheduleSchemeResult.Result != null)
					{
						dataRow.BaseSchedule = scheduleSchemeResult.Result.Name;
						dataRow.ScheduleType = scheduleSchemeResult.Result.Type.ToDescription();
					}
				}

				dataSet.Data.Rows.Add(dataRow);
			}

			return dataSet;
		}
	}
}