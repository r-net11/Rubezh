using RubezhService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RubezhService.Report.Templates
{
	public partial class EmployeeRootReport : BaseReport
	{
		public EmployeeRootReport()
		{
			InitializeComponent();
		}
		protected override bool ForcedLandscape
		{
			get { return false; }
		}
		public override string ReportTitle
		{
			get { return "Маршрут " + (GetFilter<EmployeeRootReportFilter>().IsEmployee ? "сотрудника" : "посетителя"); }
		}
		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
			CreateDynamicElements();
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeRootReportFilter>();
			var ds = new EmployeeRootDataSet();
			var employees = dataProvider.GetEmployees(filter);
			var passJournal = dataProvider.DbService.PassJournalTranslator != null ?
				dataProvider.DbService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo) : null;

			var zoneMap = new Dictionary<Guid, string>();
			if (passJournal != null)
			{
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
			}

			foreach (var employee in employees)
			{
				var employeeRow = ds.Employee.NewEmployeeRow();
				employeeRow.UID = employee.UID;
				employeeRow.Name = employee.Name;
				employeeRow.Department = employee.Department;
				employeeRow.Position = employee.Position;
				employeeRow.Organisation = employee.Organisation;

				if (!filter.IsEmployee)
				{
					var escortUID = employee.Item.EscortUID;
					var escort = escortUID.HasValue ? dataProvider.GetEmployee(escortUID.Value) : null;
					if (escort != null)
					{
						employeeRow.Escort = escort.Name;
					}
					employeeRow.Description = employee.Item.Description;
				}

				ds.Employee.AddEmployeeRow(employeeRow);
				if (passJournal != null)
				{
					var dayPassJournals = passJournal.Where(item => item.EmployeeUID == employee.UID).GroupBy(x => x.EnterTime.Date);
					var timeTrackParts = new List<RubezhDAL.DataClasses.PassJournal>();
					foreach (var dayPassJournal in dayPassJournals)
					{
						var timeTrackDayParts = NormalizePassJournals(dayPassJournal);
						timeTrackParts.AddRange(timeTrackDayParts);
					}

					foreach (var pass in timeTrackParts)
					{
						var row = ds.Data.NewDataRow();
						row.EmployeeRow = employeeRow;
						if (zoneMap.ContainsKey(pass.ZoneUID))
							row.Zone = zoneMap[pass.ZoneUID];
						if (filter.DateTimeFrom.Ticks <= pass.EnterTime.Ticks && (!pass.ExitTime.HasValue || pass.ExitTime.Value.Ticks <= filter.DateTimeTo.Ticks))
						{
							row.DateTime = new DateTime(pass.EnterTime.Ticks);
							ds.Data.AddDataRow(row);
						}
					}
				}
			}
			return ds;
		}
		public static List<RubezhDAL.DataClasses.PassJournal> NormalizePassJournals(IEnumerable<RubezhDAL.DataClasses.PassJournal> passJournals)
		{
			if (passJournals.Count() == 0)
				return new List<RubezhDAL.DataClasses.PassJournal>();

			var result = passJournals.OrderBy(x => x.EnterTime).ToList();

			for (int i = result.Count - 1; i > 0; i--)
			{
				if (result[i].EnterTime == result[i - 1].ExitTime && result[i].ZoneUID == result[i - 1].ZoneUID)
				{
					result[i].EnterTime = result[i - 1].ExitTime.Value;
					result.RemoveAt(i - 1);
				}
			}
			return result;
		}
		void CreateDynamicElements()
		{
			bool isEmployee = GetFilter<EmployeeRootReportFilter>().IsEmployee;

			EmployeeNameLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Name", (isEmployee ? "Сотрудник" : "Посетитель") + ": {0}")});

			if (isEmployee)
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
						new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Position", "Должность: {0}")});
			else
			{
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
						new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Escort", "Сопровождающий: {0}")});
			}
		}
	}
}