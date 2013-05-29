using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2.Helpers
{
	public static class BytesHelper
	{
		public static int ExtractInt(List<byte> bytes, int index)
		{
			return bytes[index + 0] * 256 * 256 * 256 + bytes[index + 1] * 256 * 256 + bytes[index + 2] * 256 + bytes[index + 3];
		}

		public static int ExtractTriple(List<byte> bytes, int index)
		{
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
	}
}