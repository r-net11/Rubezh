using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver.DataClasses
{
	public static class ExtensionMethods
	{
		public static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		public static readonly DateTime MaxYear = new DateTime(9000, 1, 1);
		public static DateTime CheckDate(this DateTime value)
		{
			if (value < MinYear)
				return MinYear;
			if (value > MaxYear)
				return MaxYear;
			return value;
		}

		public static DateTime? CheckDate(this DateTime? value)
		{
			if(value == null)
				return null;
			return value.Value.CheckDate();
		}

		public static string CheckDateSqlStr(this DateTime? value)
		{
			if (value == null)
				return "NULL";
			return "'" + value.Value.CheckDate() + "'";
		}
	}
}
