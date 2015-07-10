using System;
using FiresecAPI;

namespace SKDDriver.DataClasses
{
	public static class DbServiceHelper
	{
		public static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		public static readonly DateTime MaxYear = new DateTime(9000, 1, 1);

		public static DateTime CheckDate(DateTime value)
		{
			if (value < MinYear)
				return MinYear;
			if (value > MaxYear)
				return MaxYear;
			return value;
		}

		public static OperationResult ConcatOperationResults(params OperationResult[] results)
		{
			var result = new OperationResult();
			foreach (var item in results)
			{
				if (item.HasError)
				{
					result.HasError = true;
					result.Error = string.Format("{0} {1}", result.Error, item.Error);
				}
			}
			return result;
		}
	}
}
