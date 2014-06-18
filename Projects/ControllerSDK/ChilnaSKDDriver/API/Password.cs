using System;

namespace ChinaSKDDriverAPI
{
	public class Password
	{
		public int RecordNo { get; set; }
		public DateTime CreationDateTime { get; set; }
		public string UserID { get; set; }
		public string DoorOpenPassword { get; set; }
		public string AlarmPassword { get; set; }
		public int DoorsCount { get; set; }
		public int[] Doors { get; set; }
	}
}