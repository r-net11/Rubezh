using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientFS2.ConfigurationWriter
{
	public static class BytesHelper
	{
		public static List<byte> ShortToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).Reverse().ToList();
		}

		public static List<byte> StringToBytes(string str, int length = 20)
		{
			if (str == null)
				str = "";
			if (str.Length > length)
				str = str.Substring(0, length);
			//var bytes = Encoding.GetEncoding(1251).GetBytes(str).ToList();
			var bytes = Encoding.Default.GetBytes(str).ToList();
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
			var stringBuilder = new StringBuilder();
			if (bytes != null)
			{
				for (int index = 0; index < bytes.Count; index++)
				{
					var b = bytes[index];
					if (index % 16 == 0 && index > 0)
						stringBuilder.AppendLine("");

					stringBuilder.Append(b.ToString("x2") + " ");
				}
			}
			return stringBuilder.ToString();
		}

		public static int SubstructInt(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1] + 256 * 256 * bytes[startByte + 2] + 256 * 256 * 256 * bytes[startByte + 3];
			return result;
		}

		public static ushort SubstructShort(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1];
			return (ushort)result;
		}
	}
}