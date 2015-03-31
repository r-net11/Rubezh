using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
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

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
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
				var zoneMap = new Dictionary<Guid, string>();
				foreach (var zone in SKDManager.Zones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				var enterJournal = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesLastEnterPassJournal(
					employees.Select(item => item.UID), filter.Zones, filter.ReportDateTime);
				foreach (var record in enterJournal)
					AddRecord(dataProvider, dataSet, record, filter, true, zoneMap);
			}
			return dataSet;
		}

		private void AddRecord(DataProvider dataProvider, EmployeeZonesDataSet ds, PassJournal record, EmployeeZonesReportFilter filter, bool isEnter, Dictionary<Guid, string> zoneMap)
		{
			var dataRow = ds.Data.NewDataRow();
			var employee = dataProvider.GetEmployee(record.EmployeeUID);
			dataRow.Employee = employee.Name;
			dataRow.Orgnisation = employee.Organisation;
			dataRow.Department = employee.Department;
			dataRow.Position = employee.Position;
			dataRow.Zone = zoneMap.ContainsKey(record.ZoneUID) ? zoneMap[record.ZoneUID] : "Зона не найдена";
			dataRow.EnterDateTime = record.EnterTime;
			if (record.ExitTime.HasValue)
			{
				dataRow.ExitDateTime = record.ExitTime.Value;
				dataRow.Period = dataRow.ExitDateTime - dataRow.EnterDateTime;
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
