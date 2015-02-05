using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;

namespace FiresecService.Report.Templates
{
	public partial class Report418 : BaseReport
	{
		public Report418()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Справка о сотруднике/посетителе"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<ReportFilter418>();
			dataProvider.LoadCache();
			var employees = dataProvider.GetEmployees(filter);
			var dataSet = new DataSet418();
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.IsEmployee = filter.IsEmployee;
				dataRow.BirthDay = employee.Item.BirthDate;
				dataRow.BirthPlace = employee.Item.BirthPlace;
				dataRow.Department = employee.Department;
				dataRow.Document = employee.Item.DocumentType.ToDescription();
				dataRow.DocumentIssuer = employee.Item.DocumentGivenBy;
				dataRow.DocumentNumber = employee.Item.DocumentNumber;
				dataRow.DocumentValidFrom = employee.Item.DocumentGivenDate;
				dataRow.DocumentValidTo = employee.Item.DocumentValidTo;
				dataRow.FirstName = employee.Item.FirstName;
				dataRow.LastName = employee.Item.LastName;
				dataRow.Nationality = employee.Item.Citizenship;
				dataRow.Number = employee.Item.TabelNo;
				dataRow.Organisation = employee.Organisation;
				dataRow.Phone = employee.Item.Phone;
				if (employee.Item.Photo != null)
					dataRow.Photo = employee.Item.Photo.Data;
				dataRow.Position = employee.Position;
				dataRow.Schedule = employee.Item.Schedule == null ? employee.Item.ScheduleName : employee.Item.Schedule.Name;
				dataRow.SecondName = employee.Item.SecondName;
				dataRow.Sex = employee.Item.Gender.ToDescription();
				dataRow.UID = employee.UID;
				dataSet.Data.Rows.Add(dataRow);
				foreach (var card in employee.Item.Cards)
				{
					var cardRow = dataSet.PassCards.NewPassCardsRow();
					cardRow.DataRow = dataRow;
					cardRow.Number = card.Number;
					dataSet.PassCards.AddPassCardsRow(cardRow);
				}
				foreach (var column in employee.Item.AdditionalColumns.Where(item => item.AdditionalColumnType.DataType == AdditionalColumnDataType.Text))
				{
					var columnRow = dataSet.AdditionalColumns.NewAdditionalColumnsRow();
					columnRow.DataRow = dataRow;
					columnRow.Value = column.TextData;
					columnRow.Name = column.AdditionalColumnType.Name;
					dataSet.AdditionalColumns.AddAdditionalColumnsRow(columnRow);
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

		private void Detail_BeforePrint(object sender, PrintEventArgs e)
		{
			var currentRow = GetCurrentRow<DataSet418.DataRow>();
			xrLeftTable.BeginInit();
			for (; xrLeftTable.Rows.Count > 3; )
				xrLeftTable.Rows.RemoveAt(3);
			var cardRows = currentRow.GetPassCardsRows();
			foreach (var cardRow in cardRows)
				AddRowToLeftTable("", cardRow.Number.ToString());
			if (cardRows.Length == 0)
				AddRowToLeftTable("Пропуск", null);
			else
				xrLeftTable.Rows[3].Cells[0].Text = "Пропуск";
			foreach (var propertyRow in currentRow.GetAdditionalColumnsRows())
				AddRowToLeftTable(propertyRow.Name, propertyRow.Value);
			xrLeftTable.HeightF = xrLeftTable.Rows.Count * xrRightTable.Rows[0].HeightF;
			xrLeftTable.AdjustSize();
			xrLeftTable.EndInit();
		}
		private void AddRowToLeftTable(string text1, string text2)
		{
			var row = new XRTableRow();
			row.Cells.Add(new XRTableCell() { Text = text1, WidthF = xrLeftTable.Rows[0].Cells[0].WidthF });
			row.Cells.Add(new XRTableCell() { Text = text2, WidthF = xrLeftTable.Rows[0].Cells[1].WidthF });
			if (xrLeftTable.Rows.Count % 2 == 0)
				row.StyleName = "OddRowStyle";
			xrLeftTable.Rows.Add(row);
		}
	}
}