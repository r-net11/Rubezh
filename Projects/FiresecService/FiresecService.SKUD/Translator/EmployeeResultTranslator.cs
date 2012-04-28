using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.DataAccess;

namespace FiresecService.SKUD.Translator
{
	static class EmployeeResultTranslator
	{
		public static EmployeeCard Translate(GetAllEmployeesResult record)
		{
			return new EmployeeCard()
			{
				Id = record.Id,
				ClockNumber = record.ClockNumber,
				Department = record.Department,
				FirstName = record.FirstName,
				Group = record.Group,
				LastName = record.LastName,
				Position = record.Position,
				SecondName = record.SecondName,
			};
		}
		public static IEnumerable<EmployeeCard> Translate(IEnumerable<GetAllEmployeesResult> table)
		{
			foreach (GetAllEmployeesResult record in table)
				yield return Translate(record);
		}

		public static EmployeeCardDetails Translate(GetEmployeeCardResult record)
		{
			return new EmployeeCardDetails()
			{
				Id = record.Id,
				Address = record.Address,
				AddressFact = record.AddressFact,
				Birthday = record.Birthday,
				BirthPlace = record.BirthPlace,
				Cell = record.Cell,
				ClockNumber = record.ClockNumber,
				Comment = record.Comment,
				DepartmentId = record.DepartmentId,
				Email = record.Email,
				FirstName = record.FirstName,
				GroupId = record.GroupId,
				ITN = record.ITN,
				LastName = record.LastName,
				PassportCode = record.PassportCode,
				PassportDate = record.PassportDate,
				PassportEmitter = record.PassportEmitter,
				PassportNumber = record.PassportNumber,
				PassportSerial = record.PassportSerial,
				Phone = record.Phone,
				Photo = record.Photo == null ? null : record.Photo.ToArray(),
				PositionId = record.PositionId,
				SecondName = record.SecondName,
				SexId = record.SexId,
				SNILS = record.SNILS,
			};
		}
	}
}