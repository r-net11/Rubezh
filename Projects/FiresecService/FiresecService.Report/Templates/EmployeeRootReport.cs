using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
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

		private void CreateDetails()
		{
			float[] columnsWidth = { 600F, 0F, 1100F, 0F };

			bool isEmployee = GetFilter<EmployeeRootReportFilter>().IsEmployee;

			DevExpress.XtraReports.UI.DetailBand Detail;
			DevExpress.XtraReports.UI.XRLabel DescriptionLabel;
			DevExpress.XtraReports.UI.XRLabel PositionOrEscortLabel;
			DevExpress.XtraReports.UI.XRLabel OrganisationLabel;
			DevExpress.XtraReports.UI.XRLabel EmployeeNameLabel;
			DevExpress.XtraReports.UI.XRLabel DepartmentLabel;
			DevExpress.XtraReports.UI.DetailReportBand DetailReport;
			DevExpress.XtraReports.UI.DetailBand Detail1;
			DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
			DevExpress.XtraReports.UI.XRTable HeaderTable;
			DevExpress.XtraReports.UI.XRTableRow HeaderRow;
			DevExpress.XtraReports.UI.XRTableCell DateTimeHeaderCell;
			//DevExpress.XtraReports.UI.XRTableCell AccessPointHeaderCell;
			DevExpress.XtraReports.UI.XRTableCell ZoneHeaderCell;
			//DevExpress.XtraReports.UI.XRTableCell PassHeaderCell;
			DevExpress.XtraReports.UI.XRTable DataTable;
			DevExpress.XtraReports.UI.XRTableRow DataRow;
			DevExpress.XtraReports.UI.XRTableCell DateTimeCell;
			//DevExpress.XtraReports.UI.XRTableCell AccessPointCell;
			DevExpress.XtraReports.UI.XRTableCell ZoneCell;
			//DevExpress.XtraReports.UI.XRTableCell PassCell;
			DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;

			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeRootReport));
			Detail = new DevExpress.XtraReports.UI.DetailBand();
			PositionOrEscortLabel = new DevExpress.XtraReports.UI.XRLabel();
			OrganisationLabel = new DevExpress.XtraReports.UI.XRLabel();
			EmployeeNameLabel = new DevExpress.XtraReports.UI.XRLabel();
			DepartmentLabel = new DevExpress.XtraReports.UI.XRLabel();
			DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
			Detail1 = new DevExpress.XtraReports.UI.DetailBand();
			DataTable = new DevExpress.XtraReports.UI.XRTable();
			DataRow = new DevExpress.XtraReports.UI.XRTableRow();
			DateTimeCell = new DevExpress.XtraReports.UI.XRTableCell();
			//AccessPointCell = new DevExpress.XtraReports.UI.XRTableCell();
			ZoneCell = new DevExpress.XtraReports.UI.XRTableCell();
			//PassCell = new DevExpress.XtraReports.UI.XRTableCell();
			GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			HeaderTable = new DevExpress.XtraReports.UI.XRTable();
			HeaderRow = new DevExpress.XtraReports.UI.XRTableRow();
			DateTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			//AccessPointHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			ZoneHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			//PassHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(DataTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(HeaderTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			//
			// Detail
			//
			Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            PositionOrEscortLabel,
            OrganisationLabel,
            EmployeeNameLabel,
            DepartmentLabel});
			Detail.Dpi = 254F;
			Detail.HeightF = 152F;
			Detail.KeepTogether = true;
			Detail.KeepTogetherWithDetailReports = true;
			Detail.Name = "Detail";
			//
			// DescriptionLabal
			//
			if (!isEmployee)
			{
				DescriptionLabel = new DevExpress.XtraReports.UI.XRLabel();
				Detail.Controls.Add(DescriptionLabel);
				DescriptionLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Description", "Примечание: {0}")});
				DescriptionLabel.Dpi = 254F;
				DescriptionLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 152.40002F);
				DescriptionLabel.Name = "DescriptionLabel";
				DescriptionLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
				DescriptionLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
				DescriptionLabel.StylePriority.UseTextAlignment = false;
				DescriptionLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			}
			//
			// PositionOrEscortLabel
			//
			if (isEmployee)
			{
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Position", "Должность: {0}")});
			}
			else
			{
				PositionOrEscortLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
					new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Escort", "Сопровождающий: {0}")});
			}
			PositionOrEscortLabel.Dpi = 254F;
			PositionOrEscortLabel.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 88.90002F);
			PositionOrEscortLabel.Name = "PositionOrEscortLabel";
			PositionOrEscortLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			PositionOrEscortLabel.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			PositionOrEscortLabel.StylePriority.UseTextAlignment = false;
			PositionOrEscortLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// OrganisationLabel
			//
			OrganisationLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Organisation", "Организация: {0}")});
			OrganisationLabel.Dpi = 254F;
			OrganisationLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 88.90002F);
			OrganisationLabel.Name = "OrganisationLabel";
			OrganisationLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			OrganisationLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			OrganisationLabel.StylePriority.UseTextAlignment = false;
			OrganisationLabel.Text = "OrganisationLabel";
			OrganisationLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// EmployeeNameLabel
			//
			EmployeeNameLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Name", (isEmployee ? "Сотрудник" : "Посетитель") + ": {0}")});
			EmployeeNameLabel.Dpi = 254F;
			EmployeeNameLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 25.40002F);
			EmployeeNameLabel.Name = "EmployeeNameLabel";
			EmployeeNameLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			EmployeeNameLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			EmployeeNameLabel.StylePriority.UseTextAlignment = false;
			EmployeeNameLabel.Text = "EmployeeNameLabel";
			EmployeeNameLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// DepartmentLabel
			//
			DepartmentLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Department", "Подразделение: {0}")});
			DepartmentLabel.Dpi = 254F;
			DepartmentLabel.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 25.40002F);
			DepartmentLabel.Name = "DepartmentLabel";
			DepartmentLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			DepartmentLabel.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			DepartmentLabel.StylePriority.UseTextAlignment = false;
			DepartmentLabel.Text = "DepartmentLabel";
			DepartmentLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// DetailReport
			//
			DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            Detail1,
            GroupHeader1});
			DetailReport.DataMember = "Employee.Employee_Data";
			DetailReport.Dpi = 254F;
			DetailReport.Level = 0;
			DetailReport.Name = "DetailReport";
			DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
			DetailReport.ReportPrintOptions.DetailCountOnEmptyDataSource = 0;
			//
			// Detail1
			//
			Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            DataTable});
			Detail1.Dpi = 254F;
			Detail1.HeightF = 64F;
			Detail1.Name = "Detail1";
			Detail1.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("DateTime", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			//
			// DataTable
			//
			DataTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			DataTable.Dpi = 254F;
			DataTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			DataTable.Name = "DataTable";
			DataTable.OddStyleName = "xrControlStyle1";
			DataTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			DataTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            DataRow});
			DataTable.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			DataTable.StylePriority.UseBorders = false;
			DataTable.StylePriority.UsePadding = false;
			DataTable.StylePriority.UseTextAlignment = false;
			DataTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// DataRow
			//
			//DataRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			//DateTimeCell,
			//AccessPointCell,
			//ZoneCell,
			//PassCell});
			DataRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            DateTimeCell,
            ZoneCell});
			DataRow.Dpi = 254F;
			DataRow.Name = "DataRow";
			DataRow.Weight = 11.5D;
			//
			// DateTimeCell
			//
			DateTimeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.DateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			DateTimeCell.Dpi = 254F;
			DateTimeCell.Name = "DateTimeCell";
			DateTimeCell.StylePriority.UseTextAlignment = false;
			//DateTimeCell.Weight = 0.29D;
			DateTimeCell.WidthF = columnsWidth[0];
			//
			// AccessPointCell
			//
			//AccessPointCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			//new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.DateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			//AccessPointCell.Dpi = 254F;
			//AccessPointCell.Name = "AccessPointCell";
			//AccessPointCell.StylePriority.UseTextAlignment = false;
			//AccessPointCell.Weight = 0.23D;
			//AccessPointCell.WidthF = columnsWidth[1];
			//
			// ZoneCell
			//
			ZoneCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.Zone")});
			ZoneCell.Dpi = 254F;
			ZoneCell.Name = "ZoneCell";
			//ZoneCell.Weight = 0.29D;
			ZoneCell.WidthF = columnsWidth[2];
			//
			// PassCell
			//
			//PassCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			//new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.Zone")});
			//PassCell.Dpi = 254F;
			//PassCell.Name = "PassCell";
			//PassCell.Weight = 0.19D;
			//PassCell.WidthF = columnsWidth[3];
			//
			// GroupHeader1
			//
			GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            HeaderTable});
			GroupHeader1.Dpi = 254F;
			GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			GroupHeader1.HeightF = 64F;
			GroupHeader1.KeepTogether = true;
			GroupHeader1.Name = "GroupHeader1";
			GroupHeader1.RepeatEveryPage = true;
			//
			// HeaderTable
			//
			HeaderTable.BackColor = System.Drawing.Color.DarkGray;
			HeaderTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
			| DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			HeaderTable.Dpi = 254F;
			HeaderTable.KeepTogether = true;
			HeaderTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			HeaderTable.Name = "HeaderTable";
			HeaderTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            HeaderRow});
			HeaderTable.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			HeaderTable.StylePriority.UseBackColor = false;
			HeaderTable.StylePriority.UseBorders = false;
			HeaderTable.StylePriority.UseTextAlignment = false;
			HeaderTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// HeaderRow
			//
			HeaderRow.BackColor = System.Drawing.Color.DarkGray;
			//HeaderRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			//DateTimeHeaderCell,
			//AccessPointHeaderCell,
			//ZoneHeaderCell,
			//PassHeaderCell});
			HeaderRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			DateTimeHeaderCell,
			ZoneHeaderCell});
			HeaderRow.Dpi = 254F;
			HeaderRow.Name = "HeaderRow";
			HeaderRow.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			HeaderRow.StylePriority.UsePadding = false;
			HeaderRow.StylePriority.UseTextAlignment = false;
			HeaderRow.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			HeaderRow.Weight = 11.5D;
			//
			// DateTimeHeaderCell
			//
			DateTimeHeaderCell.Dpi = 254F;
			DateTimeHeaderCell.Name = "DateTimeHeaderCell";
			DateTimeHeaderCell.Text = "Дата/время прохода";
			//DateTimeHeaderCell.Weight = 0.29D;
			DateTimeHeaderCell.WidthF = columnsWidth[0];
			//
			// AccessPointHeaderCell
			//
			//AccessPointHeaderCell.Dpi = 254F;
			//AccessPointHeaderCell.Name = "AccessPointHeaderCell";
			//AccessPointHeaderCell.Text = "Точка доступа";
			//AccessPointHeaderCell.Weight = 0.23D;
			//AccessPointHeaderCell.WidthF = columnsWidth[1];
			//
			// ZoneHeaderCell
			//
			ZoneHeaderCell.Dpi = 254F;
			ZoneHeaderCell.Name = "ZoneHeaderCell";
			ZoneHeaderCell.Text = "Зона";
			//ZoneHeaderCell.Weight = 0.29D;
			ZoneHeaderCell.WidthF = columnsWidth[2];
			//
			// PassHeaderCell
			//
			//PassHeaderCell.Dpi = 254F;
			//PassHeaderCell.Name = "PassHeaderCell";
			//PassHeaderCell.Text = "Пропуск";
			//PassHeaderCell.Weight = 0.19D;
			//PassHeaderCell.WidthF = columnsWidth[3];
			//
			// xrControlStyle1
			//
			xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			xrControlStyle1.Name = "xrControlStyle1";
			xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			//
			// EmployeeRootReport
			//
			Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            Detail,
            DetailReport});
			DataMember = "Employee";
			DataSourceSchema = resources.GetString("$DataSourceSchema");
			ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            xrControlStyle1});
			Version = "14.1";
			Controls.SetChildIndex(DetailReport, 0);
			Controls.SetChildIndex(Detail, 0);
			((System.ComponentModel.ISupportInitialize)(DataTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(HeaderTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
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
			get { return "Маршрут " + (GetFilter<EmployeeRootReportFilter>().IsEmployee ? "сотрудника" : "посетителя"); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
			CreateDetails();
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
					if(!pass.ExitDateTime.HasValue) continue;

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