using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.DataAccess;

namespace FiresecService.SKUD.Translator
{
	static class EmployeePositionResultTranslator
	{
		public static EmployeePosition Translate(GetPositionsResult record)
		{
			return new EmployeePosition()
			{
				Id = record.Id,
				Value = record.Value,
			};
		}
		public static IEnumerable<EmployeePosition> Translate(IEnumerable<GetPositionsResult> table)
		{
			foreach (GetPositionsResult record in table)
				yield return Translate(record);
		}
	}
}