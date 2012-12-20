using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUSB
{
	public static class TimeParceHelper
	{
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

	public class BitsExtracter
	{
		List<bool> bits;
		public BitsExtracter(List<byte> bytes)
		{
			bits = new List<bool>();
			foreach (var b in bytes)
			{
				bits.Add(b.GetBit(0));
				bits.Add(b.GetBit(1));
				bits.Add(b.GetBit(2));
				bits.Add(b.GetBit(3));
				bits.Add(b.GetBit(4));
				bits.Add(b.GetBit(5));
				bits.Add(b.GetBit(6));
				bits.Add(b.GetBit(7));
			}
		}

		public int Get(int startIndex, int endIndex)
		{
			int result = 0;
			for (int i = startIndex; i <= endIndex; i++)
			{
				var boolValue = bits[i];
				var intValue = boolValue ? 1 : 0;
				result += intValue << (i - startIndex);
			}
			return result;
		}

		public override string ToString()
		{
			string timeBits = "";
			foreach (var b in bits)
			{
				var intValue = (bool)b ? 1 : 0;
				timeBits += intValue.ToString();
			}
			return timeBits;
		}
	}

	public static class BitHelper
	{
		public static bool GetBit(this byte b, int bitNumber)
		{
			return (b & (1 << bitNumber)) != 0;
		}
	}
}