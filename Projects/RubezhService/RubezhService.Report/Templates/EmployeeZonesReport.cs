using RubezhService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RubezhService.Report.Templates
{
	public partial class EmployeeZonesReport : BaseReport
	{
		public EmployeeZonesReport()
		{
			InitializeComponent();
		}
		protected override bool ForcedLandscape
		{
			get { return true; }
		}
		public override string ReportTitle
		{
			get { return "Местонахождение " + (GetFilter<EmployeeZonesReportFilter>().IsEmployee ? "сотрудников" : "посетителей"); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeZonesReportFilter>();
			if (filter.UseCurrentDate)
				filter.ReportDateTime = DateTime.Now;

			var dataSet = new EmployeeZonesDataSet();
			if (dataProvider.DbService.PassJournalTranslator != null)
			{
				var employees = dataProvider.GetEmployees(filter);
				var zoneMap = new Dictionary<Guid, string>();
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				var enterJournal = dataProvider.DbService.PassJournalTranslator.GetEmployeesLastEnterPassJournal(
					employees.Select(item => item.UID), filter.Zones, filter.ReportDateTime);
				foreach (var record in enterJournal)
					AddRecord(dataProvider, dataSet, record, filter, true, zoneMap);
			}
			return dataSet;
		}

		private void AddRecord(DataProvider dataProvider, EmployeeZonesDataSet ds, RubezhDAL.DataClasses.PassJournal record, EmployeeZonesReportFilter filter, bool isEnter, Dictionary<Guid, string> zoneMap)
		{
			if (record.EmployeeUID == null)
				return;
			var dataRow = ds.Data.NewDataRow();
			var employee = dataProvider.GetEmployee(record.EmployeeUID.Value);
			dataRow.Employee = employee.Name;
			dataRow.Orgnisation = employee.Organisation;
			dataRow.Department = employee.Department;
			dataRow.Position = employee.Position;
			dataRow.Zone = zoneMap.ContainsKey(record.ZoneUID) ? zoneMap[record.ZoneUID] : null;
			dataRow.EnterDateTime = record.EnterTime;
			if (record.ExitTime.HasValue)
			{
				dataRow.ExitDateTime = record.ExitTime.Value;
				dataRow.Period = dataRow.ExitDateTime - dataRow.EnterDateTime;
			}
			else
			{
				dataRow.ExitDateTime = filter.ReportDateTime;
				dataRow.Period = filter.ReportDateTime - dataRow.EnterDateTime;
			}

			if (!filter.IsEmployee)
			{
				var escortUID = employee.Item.EscortUID;
				if (escortUID.HasValue)
				{
					var escort = dataProvider.GetEmployee(escortUID.Value);
					dataRow.Escort = escort.Name;
				}
			}
			ds.Data.Rows.Add(dataRow);
		}
	}
}
