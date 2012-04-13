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
		public static SkudEmployee Translate(GetAllEmployeesResult record)
		{
			return new SkudEmployee()
			{
				Birthday = record.Birthday,
				Comment = record.Comment,
				FirstName = record.FirstName,
				Id = record.Id,
				LastName = record.LastNmae,
				Position = record.Position,
				SecondName = record.SecondName,
				Sex = record.Sex,
				Staff = record.Staff
			};
		}
		public static IEnumerable<SkudEmployee> Translate(IEnumerable<GetAllEmployeesResult> table)
		{
			foreach(GetAllEmployeesResult record in table)
				yield return Translate(record);
		}
	}
}
