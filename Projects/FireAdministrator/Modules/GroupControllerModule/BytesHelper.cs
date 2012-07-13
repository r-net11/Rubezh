using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule
{
	public static class BytesHelper
	{
		public static List<byte> ShortToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static List<byte> StringDescriptionToBytes(string str, int length = 32)
		{
			if (str.Length > length)
				str = str.Substring(0, length);
			var bytes = Encoding.GetEncoding(1251).GetBytes(str).ToList();
			for (int i = 0; i < length - str.Length; i++)
			{
				bytes.Add(32);
			}
			return bytes;
		}
	}
}