using System;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriverAPI
{
	public class Card
	{
		public int RecordNo { get; set; }
		public string CardNo { get; set; }
		public NativeWrapper.NET_ACCESSCTLCARD_TYPE CardType { get; set; }
		public string Password { get; set; }
		public int DoorsCount { get; set; }
		public int[] Doors { get; set; }
		public int TimeSectionsCount { get; set; }
		public int[] TimeSections { get; set; }
		public int UserTime { get; set; }
		public DateTime ValidStartDateTime { get; set; }
		public DateTime ValidEndDateTime { get; set; }
	}
}