using System;

namespace FiresecService
{
	static class BoolExtentions
	{
		public static string ToYesNo(this Boolean value)
		{
			return value ? "Да" : "Нет";
		}
	}
}
