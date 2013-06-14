using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerFS2
{
	public static class BytesHelper
	{
		public static bool IsRevese { get; set; }

		public static List<byte> ShortToBytes(short shortValue)
		{
			if (IsRevese)
			{
				return BitConverter.GetBytes(shortValue).ToList();
			}
			else
			{
				return BitConverter.GetBytes(shortValue).Reverse().ToList();
			}
		}

		public static List<byte> IntToBytes(int intValue)
		{
			if (IsRevese)
			{
				return BitConverter.GetBytes(intValue).ToList();
			}
			else
			{
				return BitConverter.GetBytes(intValue).Reverse().ToList();
			}
		}

		public static List<byte> StringToBytes(string str, int length = 20)
		{
			//var bytes1 = new List<byte>();
			//bytes1.Add(66);
			//bytes1.Add(85);
			//bytes1.Add(78);
			//bytes1.Add(83);
			//bytes1.Add(53);
			//bytes1.Add(32);
			//bytes1.Add(204);
			//bytes1.Add(207);
			//bytes1.Add(210);
			//bytes1.Add(45);
			//bytes1.Add(49);
			//bytes1.Add(32);
			//bytes1.Add(48);
			//bytes1.Add(46);
			//bytes1.Add(49);
			//bytes1.Add(46);
			//bytes1.Add(49);
			//bytes1.Add(54);
			//var description = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);

			if (str == null)
				str = "";
			if (str.Length > length)
				str = str.Substring(0, length);
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

		public static int ExtractInt(List<byte> bytes, int index)
		{
			return bytes[index + 0] * 256 * 256 * 256 + bytes[index + 1] * 256 * 256 + bytes[index + 2] * 256 + bytes[index + 3];
		}

		public static int ExtractTriple(List<byte> bytes, int index)
		{
			if (index < 0)
				return 0;
			return bytes[index + 0] * 256 * 256 + bytes[index + 1] * 256 + bytes[index + 2];
		}

		public static int ExtractShort(List<byte> bytes, int index)
		{
			return bytes[index + 0] * 256 + bytes[index + 1];
		}

		public static string ExtractString(List<byte> bytes, int index)
		{
			return new string(Encoding.Default.GetChars(bytes.GetRange(index, 20).ToArray())).TrimEnd(' ');
		}

		public static void BytesToFile(string fileName, List<byte> bytes)
		{
			var file = new StreamWriter("..\\" + fileName);
			foreach (var b in bytes)
			{
				file.Write("{0} ", b.ToString("X2"));
			}
			file.Close();
		}

		public static List<byte> BytesFromFile(string fileName)
		{
			var bytes = new List<byte>();
			return bytes;
		}
	}
}