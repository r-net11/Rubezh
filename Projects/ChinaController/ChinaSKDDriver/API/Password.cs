using System;
using System.Collections.Generic;

namespace StrazhDeviceSDK.API
{
	public class Password
	{
		public Password()
		{
			Doors = new List<int>();
		}

		public int RecordNo { get; set; }

		public DateTime CreationDateTime { get; set; }

		public string UserID { get; set; }

		public string DoorOpenPassword { get; set; }

		public string AlarmPassword { get; set; }

		public int DoorsCount { get; set; }

		public List<int> Doors { get; set; }
	}
}