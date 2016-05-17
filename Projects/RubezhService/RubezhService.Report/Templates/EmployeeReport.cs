using System.Data;
using System.Drawing.Printing;
using DevExpress.XtraReports.UI;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhService.Report.DataSources;
using System.Linq;

namespace RubezhService.Report.Templates
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
		public override string ReportTitle
		{
			get { return "Справка о " + (GetFilter<EmployeeReportFilter>().IsEmployee ? "сотруднике" : "посетителе"); }
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
            var cards = dataProvider.GetCards(new CardFilter { EmployeeFilter = new EmployeeFilter { UIDs = employees.Select(x => x.UID).ToList() } });
            var dataSet = new EmployeeDataSet();
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
				int number = -1;
				int.TryParse(employee.Item.TabelNo, out number);
				dataRow.Number = number;
				dataRow.Organisation = employee.Organisation;
				dataRow.Phone = employee.Item.Phone;
				if (employee.Item.Photo != null)
					dataRow.Photo = employee.Item.Photo.Data;
				if (employee.Item.Type == PersonType.Guest)
				{
					dataRow.Type = "Сопровождающий";
                    dataRow.PositionOrEscort = employee.Item.EscortName;
				}
				if (employee.Item.Type == PersonType.Employee)
				{
					dataRow.Type = "Должность";
					dataRow.PositionOrEscort = employee.Position;
					dataRow.Schedule = employee.Item.ScheduleName;
				}
				dataRow.SecondName = employee.Item.SecondName;
				dataRow.Sex = employee.Item.Gender.ToDescription();
				dataRow.UID = employee.UID;
				dataSet.Data.Rows.Add(dataRow);
                var employeeCards = cards.Where(x => x.EmployeeUID == employee.UID);
                foreach (var card in employeeCards)
				{
					var cardRow = dataSet.PassCards.NewPassCardsRow();
					cardRow.DataRow = dataRow;
					cardRow.Number = (int)card.Number;
					dataSet.PassCards.AddPassCardsRow(cardRow);
				}
				foreach (var column in employee.Item.AdditionalColumns)
				{
					if (column.DataType == AdditionalColumnDataType.Text)
					{
						var columnRow = dataSet.AdditionalColumns.NewAdditionalColumnsRow();
						columnRow.DataRow = dataRow;
						columnRow.Value = column.TextData;
						columnRow.Name = column.ColumnName;
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
			if (!GetFilter<EmployeeReportFilter>().IsEmployee)
				row.BackColor = System.Drawing.Color.LightGray;
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