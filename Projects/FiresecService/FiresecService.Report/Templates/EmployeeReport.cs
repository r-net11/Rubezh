using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Data;
using System.Drawing.Printing;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeReport : BaseReport
	{
		public EmployeeReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return false; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.InfoAbout + (GetFilter<EmployeeReportFilter>().IsEmployee ? CommonResources.EmployeeAbout : CommonResources.VisitorAbout); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeReportFilter>();
			dataProvider.LoadCache();
			var employees = dataProvider.GetEmployees(filter);
			var dataSet = new EmployeeDataSet();
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.IsEmployee = filter.IsEmployee;
				dataRow.BirthDay = employee.Item.BirthDate.HasValue
					? employee.Item.BirthDate.Value.ToString("d")
					: String.Empty;
				dataRow.BirthPlace = employee.Item.BirthPlace;
				dataRow.Department = employee.Department;
				dataRow.Document = employee.Item.DocumentType.ToDescription();
				dataRow.DocumentIssuer = employee.Item.DocumentGivenBy;
				dataRow.DocumentNumber = employee.Item.DocumentNumber;
				dataRow.DocumentValidFrom = employee.Item.DocumentGivenDate.HasValue
					? employee.Item.DocumentGivenDate.Value.ToString("d")
					: String.Empty;
				dataRow.DocumentValidTo = employee.Item.DocumentValidTo.HasValue
					? employee.Item.DocumentValidTo.Value.ToString("d")
					: String.Empty;
				dataRow.FirstName = employee.Item.FirstName;
				dataRow.LastName = employee.Item.LastName;
				dataRow.Nationality = employee.Item.Citizenship;
				int number = -1;
				int.TryParse(employee.Item.TabelNo, out number);
				dataRow.Number = number;
				dataRow.Organisation = employee.Organisation;
				dataRow.Phone = employee.Item.Phone;
				if (employee.Item.Photo != null)
					dataRow.Photo = employee.Item.Photo.Data;
				dataRow.Position = employee.Position;
				dataRow.Schedule = employee.Item.Schedule == null || employee.Item.Schedule.IsDeleted ? null : employee.Item.Schedule.Name;
				dataRow.SecondName = employee.Item.SecondName;
				dataRow.Sex = employee.Item.Gender.ToDescription();
				dataRow.UID = employee.UID;
				dataSet.Data.Rows.Add(dataRow);
				foreach (var card in employee.Item.Cards)
				{
					var cardRow = dataSet.PassCards.NewPassCardsRow();
					cardRow.DataRow = dataRow;
					if (card.Number != null)
						cardRow.Number = (int)card.Number;
					dataSet.PassCards.AddPassCardsRow(cardRow);
				}
				foreach (var column in employee.Item.AdditionalColumns)
				{
					if (column.AdditionalColumnType != null && column.AdditionalColumnType.DataType == AdditionalColumnDataType.Text)
					{
						var columnRow = dataSet.AdditionalColumns.NewAdditionalColumnsRow();
						columnRow.DataRow = dataRow;
						columnRow.Value = column.TextData;
						columnRow.Name = column.AdditionalColumnType.Name;
						dataSet.AdditionalColumns.AddAdditionalColumnsRow(columnRow);
					}
				}
			}
			return dataSet;
		}

		protected override void ApplySort()
		{
			if (string.IsNullOrEmpty(Filter.SortColumn) || Filter.SortColumn == "Employee")
			{
				Detail.SortFields.Add(new GroupField("LastName", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
				Detail.SortFields.Add(new GroupField("FirstName", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
				Detail.SortFields.Add(new GroupField("SecondName", Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
			}
			else
				base.ApplySort();
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.xrTableCell37.Text = CommonResources.PersonnelNumber;
			this.xrTableCell39.Text = CommonResources.Phone;
			this.xrTableCell35.Text = CommonResources.WorkSchedule;
			this.xrTableCell1.Text = CommonResources.LastName;
			this.xrTableCell3.Text = CommonResources.FirstName;
			this.xrTableCell9.Text = CommonResources.SecondName;
			this.xrTableCell7.Text = CommonResources.Organization;
			this.xrTableCell5.Text = CommonResources.Department;
			this.xrTableCell11.Text = CommonResources.Position;
			this.xrTableCell13.Text = CommonResources.Document;
			this.xrTableCell15.Text = CommonResources.Number;
			this.xrTableCell17.Text = CommonResources.Citizenship;
			this.xrTableCell19.Text = CommonResources.Sex;
			this.xrTableCell23.Text = CommonResources.BirthDate;
			this.xrTableCell27.Text = CommonResources.BirthPlace;
			this.xrTableCell29.Text = CommonResources.IssuedBy;
			this.xrTableCell31.Text = CommonResources.IssueDate;
			this.xrTableCell33.Text = CommonResources.ExpireDate;
			this.EmployeeText.Expression = string.Format("Iif([IsEmployee] == True, \'{0}\', \'{1}\')", CommonResources.InfoAboutEmployee, CommonResources.InfoAboutVisitor);
		}

		private void Detail_BeforePrint(object sender, PrintEventArgs e)
		{
			var currentRow = GetCurrentRow<EmployeeDataSet.DataRow>();
			xrLeftTable.BeginInit();
			for (; xrLeftTable.Rows.Count > 3;)
				xrLeftTable.Rows.RemoveAt(3);
			var cardRows = currentRow.GetPassCardsRows();
			foreach (var cardRow in cardRows)
				AddRowToLeftTable("", cardRow.Number.ToString());
			if (cardRows.Length == 0)
				AddRowToLeftTable(CommonResources.Passcard, null);
			else
				xrLeftTable.Rows[3].Cells[0].Text = CommonResources.Passcard;
			foreach (var propertyRow in currentRow.GetAdditionalColumnsRows())
				AddRowToLeftTable(propertyRow.Name, propertyRow.Value);
			xrLeftTable.HeightF = xrLeftTable.Rows.Count * xrRightTable.Rows[0].HeightF;
			xrLeftTable.AdjustSize();
			xrLeftTable.EndInit();
		}

		private void AddRowToLeftTable(string text1, string text2)
		{
			var row = new XRTableRow();
			if (xrLeftTable.Rows.Count % 2 == 0)
				row.StyleName = "OddRowStyle";
			xrLeftTable.Rows.Add(row);
			row.Cells.Add(new XRTableCell() { Text = text1 });
			row.Cells.Add(new XRTableCell() { Text = text2 });
			row.Cells[0].WidthF = WidthF = xrLeftTable.Rows[0].Cells[0].WidthF;
			row.Cells[1].WidthF = WidthF = xrLeftTable.Rows[0].Cells[1].WidthF;
		}
	}
}