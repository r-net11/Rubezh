using System.Collections.Generic;
using System.Data;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeReport : BaseReport<EmployeeReportData>
	{
		public override EmployeeReportData CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeReportFilter>(f);
			dataProvider.LoadCache();
			var employees = dataProvider.GetEmployees(filter);
			var cards = dataProvider.GetCards(new CardFilter { EmployeeFilter = new EmployeeFilter { UIDs = employees.Select(x => x.UID).ToList() } });
			var result = new EmployeeReportData();
			result.EmployeeData = new List<EmployeeData>();
			result.AdditionalColumns = new List<EmployeeAdditionalColumnsData>();
			result.PassCards = new List<EmployeePassCardData>();
			foreach (var employee in employees)
			{
				var data = new EmployeeData();
				data.IsEmployee = filter.IsEmployee;
				data.BirthDay = employee.Item.BirthDate;
				data.BirthPlace = employee.Item.BirthPlace;
				data.Department = employee.Department;
				data.Document = employee.Item.DocumentType.ToDescription();
				data.DocumentIssuer = employee.Item.DocumentGivenBy;
				data.DocumentNumber = employee.Item.DocumentNumber;
				data.DocumentValidFrom = employee.Item.DocumentGivenDate;
				data.DocumentValidTo = employee.Item.DocumentValidTo;
				data.FirstName = employee.Item.FirstName;
				data.LastName = employee.Item.LastName;
				data.Nationality = employee.Item.Citizenship;
				int number = -1;
				int.TryParse(employee.Item.TabelNo, out number);
				data.Number = number;
				data.Organisation = employee.Organisation;
				data.Phone = employee.Item.Phone;
				if (employee.Item.Photo != null)
					data.Photo = employee.Item.Photo.Data;
				if (employee.Item.Type == PersonType.Guest)
				{
					data.Type = "Сопровождающий";
					data.PositionOrEscort = employee.Item.EscortName;
				}
				if (employee.Item.Type == PersonType.Employee)
				{
					data.Type = "Должность";
					data.PositionOrEscort = employee.Position;
					data.Schedule = employee.Item.ScheduleName;
				}
				data.SecondName = employee.Item.SecondName;
				data.Sex = employee.Item.Gender.ToDescription();
				data.UID = employee.UID;
				result.EmployeeData.Add(data);
				var employeeCards = cards.Where(x => x.EmployeeUID == employee.UID);
				foreach (var card in employeeCards)
				{
					EmployeePassCardData cardData = new EmployeePassCardData();
					cardData.EmployeeUID = data.UID;
					cardData.Number = (int)card.Number;
					result.PassCards.Add(cardData);
				}
				foreach (var column in employee.Item.AdditionalColumns)
				{
					if (column.DataType == AdditionalColumnDataType.Text)
					{
						EmployeeAdditionalColumnsData columnData = new EmployeeAdditionalColumnsData();
						columnData.EmployeeUID = data.UID;
						columnData.Value = column.TextData;
						columnData.Name = column.ColumnName;
						result.AdditionalColumns.Add(columnData);
					}
				}
			}
			return result;
		}
	}
}
