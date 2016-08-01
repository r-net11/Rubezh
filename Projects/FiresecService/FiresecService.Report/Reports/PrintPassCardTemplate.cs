using StrazhAPI.Printing;
using StrazhAPI.SKD;
using System.Collections.Generic;

namespace FiresecService.Report.Reports
{
	//public class PrintPassCardTemplate : Base<List<EmployeeDTO>>
	//{
	//	public override List<EmployeeDTO> CreateDataSet(DataProvider dataProvider, OrganisationFilterBase filter)
	//	{
	//		var currentFilter = GetFilter<EmployeeFilter>(filter);
	//		dataProvider.LoadCache();
	//		var employees = dataProvider.GetEmployees(currentFilter, false);
	//		var result = new List<EmployeeDTO>();

	//		foreach (var el in employees)
	//		{
	//			var row = new EmployeeDTO
	//			{
	//				BirthDate = el.Item.BirthDate,
	//				BirthPlace = el.Item.BirthPlace,
	//				Citizenship = el.Item.Citizenship,
	//				DepartmentUID = el.DepartmentUID,
	//				Description = el.Item.Description,
	//				DocumentDepartmentCode = el.Item.DocumentDepartmentCode,
	//				DocumentGivenBy = el.Item.DocumentGivenBy,
	//				DocumentGivenDate = el.Item.DocumentGivenDate,
	//				DocumentNumber = el.Item.DocumentNumber,
	//				DocumentType = el.Item.DocumentType,
	//				DocumentValidTo = el.Item.DocumentValidTo,
	//				EscortUID = el.Item.EscortUID,
	//				ExternalKey = "-1",
	//				FirstName = el.Item.FirstName,
	//				Gender = el.Item.Gender,
	//				IsDeleted = el.Item.IsDeleted,
	//				LastEmployeeDayUpdate = el.Item.LastEmployeeDayUpdate,
	//				LastName = el.Item.LastName,
	//				OrganisationUID = el.OrganisationUID,
	//				Phone = el.Item.Phone,
	//				PhotoUID = el.Item.Photo.UID,
	//				PositionUID = el.Item.Position.UID,
	//				RemovalDate = el.Item.RemovalDate,
	//				ScheduleStartDate = el.Item.ScheduleStartDate,
	//				SecondName = el.Item.SecondName,
	//				TableNo = null,
	//				Type = el.Item.Type,
	//				UID = el.Item.UID
	//			};

	//			result.Add(row);
	//		}

	//		return result;
	//	}
	//}
}
