using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver.DataAccess;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeZonesReport : BaseReport
	{
		public EmployeeZonesReport()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Местонахождение сотрудников (посетителей)"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeZonesReportFilter>();
			if (filter.UseCurrentDate)
				filter.ReportDateTime = DateTime.Now;

			var dataSet = new EmployeeZonesDataSet();
			if (dataProvider.DatabaseService.PassJournalTranslator != null)
			{
				var employees = dataProvider.GetEmployees(filter);
				var enterJournal = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesLastEnterPassJournal(employees.Select(item => item.UID), filter.Zones, filter.UseCurrentDate ? (DateTime?)null : filter.ReportDateTime);
				var exitJournal = filter.Zones.IsEmpty() ? dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesLastExitPassJournal(employees.Select(item => item.UID).Except(enterJournal.Select(item => item.EmployeeUID)), filter.UseCurrentDate ? (DateTime?)null : filter.ReportDateTime) : null;
				var zoneMap = SKDManager.Zones.ToDictionary(item => item.UID);
				foreach (var record in enterJournal)
					AddRecord(dataProvider, dataSet, record, filter, true, zoneMap);
				if (exitJournal != null)
					foreach (var record in exitJournal)
						AddRecord(dataProvider, dataSet, record, filter, false, zoneMap);
			}
			return dataSet;
		}

		private void AddRecord(DataProvider dataProvider, EmployeeZonesDataSet ds, PassJournal record, EmployeeZonesReportFilter filter, bool isEnter, Dictionary<Guid, SKDZone> zoneMap)
		{
			var dataRow = ds.Data.NewDataRow();
			var employee = dataProvider.GetEmployee(record.EmployeeUID);
			dataRow.Employee = employee.Name;
			dataRow.Orgnisation = employee.Organisation;
			dataRow.Department = employee.Department;
			dataRow.Position = employee.Position;

			dataRow.EnterDateTime = isEnter ? record.EnterTime : record.ExitTime.Value;
			if (isEnter && zoneMap.ContainsKey(record.ZoneUID))
				dataRow.Zone = zoneMap[record.ZoneUID].PresentationName;
			if (filter.UseCurrentDate)
				dataRow.Period = filter.ReportDateTime - dataRow.EnterDateTime;
			else
			{
				dataRow.Period = (!isEnter && record.ExitTime.HasValue ? record.ExitTime.Value : filter.ReportDateTime) - dataRow.EnterDateTime;
				if (!isEnter && record.ExitTime.HasValue)
					dataRow.ExitDateTime = record.ExitTime.Value;
			}
			ds.Data.Rows.Add(dataRow);
		}

		private void Report417_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			var filter = GetFilter<EmployeeZonesReportFilter>();
			if (filter.UseCurrentDate)
			{
				xrTableHeader.BeginInit();
				xrTableHeader.DeleteColumn(xrTableCellExitHeader);
				xrTableHeader.EndInit();
				xrTableContent.BeginInit();
				xrTableContent.DeleteColumn(xrTableCellExit);
				xrTableContent.EndInit();
			}
		}
	}
}