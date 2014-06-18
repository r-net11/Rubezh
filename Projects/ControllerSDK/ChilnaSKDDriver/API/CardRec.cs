using System;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriverAPI
{
	public class CardRec
	{
		public int RecordNo { get; set; }
		public string CardNo { get; set; }
		public string Password { get; set; }
		public DateTime DateTime { get; set; }
		public bool IsStatus { get; set; }
		public NativeWrapper.NET_ACCESS_DOOROPEN_METHOD DoorOpenMethod { get; set; }
		public int DoorNo { get; set; }
	}
}