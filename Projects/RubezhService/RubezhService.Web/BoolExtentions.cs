using System;

namespace RubezhService
{
	public static class BoolExtentions
	{
		public static string ToYesNo(this Boolean value)
		{
			return value ? "Да" : "Нет";
		}
	}
}
