using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using StrazhDAL.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeRootReport : BaseReport
	{
		public EmployeeRootReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Портретная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return false; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.Route + (GetFilter<EmployeeRootReportFilter>().IsEmployee ? CommonResources.EmployeeNo : CommonResources.VisitorNo); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeRootReportFilter>();
			var ds = new EmployeeRootDataSet();
			var employees = dataProvider.GetEmployees(filter);
			var passJournal =
				dataProvider.DatabaseService.PassJournalTranslator != null
				? dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo)
				: null;

			var zoneMap = ZoneMap(passJournal);

			foreach (var employee in employees)
			{
				var employeeRow = ds.Employee.NewEmployeeRow();
				employeeRow.UID = employee.UID;
				employeeRow.Name = employee.Name;
				employeeRow.Department = employee.Department;
				employeeRow.Position = employee.Position;
				employeeRow.Organisation = employee.Organisation;

				// Для посетителя получить дополнительные данные
				if (!filter.IsEmployee)
				{
					// Сопровождающий
					var escort = dataProvider.GetEmployee(employee.Item.EscortUID.GetValueOrDefault());

					if (escort != null)
					{
						employeeRow.Escort = escort.Name;
					}
					// Примечание
					employeeRow.Description = employee.Item.Description;
				}

				ds.Employee.AddEmployeeRow(employeeRow);

				if (passJournal == null) continue;

				foreach (var pass in GetTimeTrackParts(GetDayPassJournals(passJournal, employee)))
				{
					if (!pass.ExitDateTime.HasValue) continue;

					var row = ds.Data.NewDataRow();
					row.EmployeeRow = employeeRow;
					if (zoneMap.ContainsKey(pass.ZoneUID))
						row.Zone = zoneMap[pass.ZoneUID];
					if (filter.DateTimeFrom.Ticks <= pass.EnterDateTime.Ticks && pass.ExitDateTime.Value.Ticks <= filter.DateTimeTo.Ticks)
					{
						row.DateTime = new DateTime(pass.EnterDateTime.Ticks);
						ds.Data.AddDataRow(row);
					}
				}
			}
			return ds;
		}

		public IEnumerable<IGrouping<DateTime, PassJournal>> GetDayPassJournals(IEnumerable<PassJournal> passJournal, EmployeeInfo employee)
		{
			return passJournal.Where(item => item.EmployeeUID == employee.UID).GroupBy(x => x.EnterTime.Date);
		}

		public List<TimeTrackPart> GetTimeTrackParts(IEnumerable<IGrouping<DateTime, PassJournal>> dayPassJournals)
		{
			var timeTrackParts = new List<TimeTrackPart>();
			foreach (var item in dayPassJournals)
			{
				timeTrackParts.AddRange(item.Select(pass => new TimeTrackPart
				{
					EnterDateTime = pass.EnterTime,//new TimeSpan(pass.EnterTime.Ticks),
					ExitDateTime = pass.ExitTime.HasValue ? pass.ExitTime.Value : new DateTime(),//new TimeSpan(pass.ExitTime.HasValue ? pass.ExitTime.Value.Ticks : 0),
					PassJournalUID = pass.UID,
					ZoneUID = pass.ZoneUID
				}));
			}
			return timeTrackParts;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();
			bool isEmployee = GetFilter<EmployeeRootReportFilter>().IsEmployee;

			if (isEmployee)
			{
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Position", CommonResources.PositionWithColon)});
			}
			else
			{
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Escort", CommonResources.MaintainerWithColon)});

				DescriptionLabel = new DevExpress.XtraReports.UI.XRLabel();
				Detail.Controls.Add(DescriptionLabel);
				DescriptionLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Description", CommonResources.NoteWithColon)});
				DescriptionLabel.Dpi = 254F;
				DescriptionLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 152.40002F);
				DescriptionLabel.Name = "DescriptionLabel";
				DescriptionLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
				DescriptionLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
				DescriptionLabel.StylePriority.UseTextAlignment = false;
				DescriptionLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			}

			EmployeeNameLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Name", (isEmployee ? CommonResources.Employee : CommonResources.Visitor) + ": {0}")});
			OrganisationLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Organisation", CommonResources.OrganizationWithColon)});
			DateTimeHeaderCell.Text = CommonResources.PassDate;
			ZoneHeaderCell.Text = CommonResources.Zone;
		}

		private Dictionary<Guid, string> ZoneMap(IEnumerable<PassJournal> passJournal)
		{
			var zoneMap = new Dictionary<Guid, string>();
			if (passJournal != null)
			{
				foreach (var zone in SKDManager.Zones)
				{
					zoneMap.Add(zone.UID, zone.Name);
				}
			}
			return zoneMap;
		}
	}
}