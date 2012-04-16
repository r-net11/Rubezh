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
		public static EmployeeCardIndex Translate(GetAllEmployeesResult record)
		{
			return new EmployeeCardIndex()
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
		public static IEnumerable<EmployeeCardIndex> Translate(IEnumerable<GetAllEmployeesResult> table)
		{
			foreach(GetAllEmployeesResult record in table)
				yield return Translate(record);
		}
	}
}
