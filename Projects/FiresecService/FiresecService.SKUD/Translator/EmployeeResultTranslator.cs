using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				PersonId = record.PersonId,
				LastName = record.LastName,
				FirstName = record.FirstName,
				SecondName = record.SecondName,
				Age = record.Age,
				Department = record.Department,
				Position = record.Position,
				Comment = record.Comment,
			};
		}
		public static IEnumerable<EmployeeCard> Translate(IEnumerable<GetAllEmployeesResult> table)
		{
			foreach(GetAllEmployeesResult record in table)
				yield return Translate(record);
		}

		public static EmployeeCardDetails Translate(GetEmployeeCardResult record)
		{
			return new EmployeeCardDetails()
			{
				Id = record.Id,
				PersonId = record.PersonId,
				LastName = record.LastName,
				FirstName = record.FirstName,
				SecondName = record.SecondName,
				Age = record.Age,
				Department = record.Department,
				Position = record.Position,
				Comment = record.Comment,
			};
		}
	}
}
