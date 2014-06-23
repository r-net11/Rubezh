using System;

namespace ChinaSKDDriverAPI
{
	public class Holiday
	{
		public int RecordNo { get; set; }
		public int DoorsCount { get; set; }
		public int[] Doors { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
		public bool IsEnabled { get; set; }
	}
}