using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.GK;

namespace Commom.GK
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

		public static string BytesToStringDescription(List<byte> bytes)
		{
			var encoding = Encoding.GetEncoding(1251);
			string result = encoding.GetString(bytes.ToArray());
			return result;
		}

		public static string BytesToString(List<byte> bytes)
		{
			var stringValue = new StringBuilder();
			if (bytes != null)
			{
				foreach (var b in bytes)
				{
					stringValue.Append(b.ToString("x2") + " ");
				}
			}
			return stringValue.ToString();
		}

		public static int SubstructInt(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1] + 256 * 256 * bytes[startByte + 2] + 256 * 256 * 256 * bytes[startByte + 3];
			return result;
		}

		public static short SubstructShort(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1];
			return (short)result;
		}
	}
}