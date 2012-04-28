using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.DataAccess;

namespace FiresecService.SKUD.Translator
{
	static class EmployeeGroupResultTranslator
	{
		public static EmployeeGroup Translate(GetGroupsResult record)
		{
			return new EmployeeGroup()
			{
				Id = record.Id,
				Value = record.Value
			};
		}
		public static IEnumerable<EmployeeGroup> Translate(IEnumerable<GetGroupsResult> table)
		{
			foreach (GetGroupsResult record in table)
				yield return Translate(record);
		}
	}
}