using System.Collections.Generic;
using System;

namespace ServerFS2
{
	public static class TimeParceHelper
	{
		public static DateTime ParceDateTime(List<byte> bytes)
		{
			var bitsExtracter = new BitsExtracter(bytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);
			var resultString = day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
			DateTime result;
			if (DateTime.TryParse(resultString, out result))
				return result;
			else
				return DateTime.Now;
		}

		public static string Parce(List<byte> bytes)
		{
			var bitsExtracter = new BitsExtracter(bytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);

			var result = day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
			return result;
		}
	}
}