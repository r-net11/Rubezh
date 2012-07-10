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
	}
}