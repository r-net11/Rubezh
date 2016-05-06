using System;
using System.Collections.Generic;

namespace StrazhDeviceSDK.API
{
	public class Holiday
	{
		public Holiday()
		{
			Doors = new List<int>();
		}

		public int RecordNo { get; set; }

		public int DoorsCount { get; set; }

		public List<int> Doors { get; set; }

		public DateTime StartDateTime { get; set; }

		public DateTime EndDateTime { get; set; }

		public string HolidayNo { get; set; }
	}
}