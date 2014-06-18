using System;
using System.Linq;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		#region Common
		public static string CharArrayToString(char[] charArray)
		{
			var result = new string(charArray);
			int i = result.IndexOf('\0');
			if (i >= 0)
				result = result.Substring(0, i);
			return result;
		}

		public static string CharArrayToStringNoTrim(char[] charArray)
		{
			var result = new string(charArray);
			//int i = result.IndexOf('\0');
			//if (i >= 0)
			//    result = result.Substring(0, i);
			return result;
		}

		public static char[] StringToCharArray(string str, int size)
		{
			var result = new char[size];
			var charArray = str.ToCharArray();
			for (int i = 0; i < Math.Min(charArray.Count(), size); i++)
			{
				result[i] = charArray[i];
			}
			return result;
		}

		public static DateTime NET_TIMEToDateTime(NativeWrapper.NET_TIME netTime)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				if (netTime.dwYear == 0 || netTime.dwMonth == 0 || netTime.dwDay == 0)
					return new DateTime();
				dateTime = new DateTime(netTime.dwYear, netTime.dwMonth, netTime.dwDay, netTime.dwHour, netTime.dwMinute, netTime.dwSecond);
			}
			catch { }
			return dateTime;
		}
		#endregion

	}
}