using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.DataAccess;

namespace FiresecService.SKUD.Translator
{
	static class EmployeeDepartmentResultTranslator
	{
		public static EmployeeDepartment Translate(GetDepartmentsResult record)
		{
			return new EmployeeDepartment()
			{
				Id = record.Id,
				Value = record.Value,
				ParentId = record.DepartmentId
			};
		}
		public static IEnumerable<EmployeeDepartment> Translate(IEnumerable<GetDepartmentsResult> table)
		{
			foreach (GetDepartmentsResult record in table)
				yield return Translate(record);
		}
	}
}