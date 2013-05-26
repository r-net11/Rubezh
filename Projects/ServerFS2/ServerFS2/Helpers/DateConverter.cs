using System;
using System.Collections.Generic;

namespace ServerFS2.Helpers
{
	public static class DateConverter
	{
		public static List<byte> ConvertToBytes(DateTime date)
		{
			var arr = Convert.ToString(date.Day, 2).PadLeft(5, '0').ToCharArray();
			Array.Reverse(arr);
			var day = new string(arr);
			arr = Convert.ToString(date.Month, 2).PadLeft(4, '0').ToCharArray();
			Array.Reverse(arr);
			var month = new string(arr);
			arr = Convert.ToString(date.Year - 2000, 2).PadLeft(6, '0').ToCharArray();
			Array.Reverse(arr);
			var year = new string(arr);
			arr = Convert.ToString(date.Hour, 2).PadLeft(5, '0').ToCharArray();
			Array.Reverse(arr);
			var hour = new string(arr);
			arr = Convert.ToString(date.Minute, 2).PadLeft(6, '0').ToCharArray();
			Array.Reverse(arr);
			var minute = new string(arr);
			arr = Convert.ToString(date.Second, 2).PadLeft(6, '0').ToCharArray();
			Array.Reverse(arr);
			var second = new string(arr);
			var binstring = day + month + year + hour + minute + second;
			var bytes = new List<byte>();
			for (int i = 0; i < 4; ++i)
			{
				arr = binstring.Substring(8 * i, 8).ToCharArray();
				Array.Reverse(arr);
				bytes.Add(Convert.ToByte(new string(arr), 2));
			}
			return bytes;
		}

		public static DateTime ConvertFromBytes(List<byte> timeBytes)
		{
			var bitsExtracter = new BitsExtracter(timeBytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = 2000 + bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var minute = bitsExtracter.Get(20, 25);
			var second = bitsExtracter.Get(26, 31);
			return new DateTime(year, month, day, hour, minute, second);
		}
	}
}