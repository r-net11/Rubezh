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
				Birthday = record.Birthday,
				Comment = record.Comment,
				FirstName = record.FirstName,
				Id = record.Id,
				LastName = record.LastName,
				Position = record.Position,
				SecondName = record.SecondName,
				Sex = record.Sex,
				Staff = record.Staff
			};
		}
		public static IEnumerable<EmployeeCard> Translate(IEnumerable<GetAllEmployeesResult> table)
		{
			foreach(GetAllEmployeesResult record in table)
				yield return Translate(record);
		}
	}
}
