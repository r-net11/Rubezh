using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeReportFilter>(f);
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
	}
}
