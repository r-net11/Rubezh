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
		public CardRecDoorOpenMethod DoorOpenMethod { get; set; }
		public int DoorNo { get; set; }
	}

	public enum CardRecDoorOpenMethod
	{
		NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,
		NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,
		NET_ACCESS_DOOROPEN_METHOD_CARD,
		NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,
		NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,
		NET_ACCESS_DOOROPEN_METHOD_REMOTE,
		NET_ACCESS_DOOROPEN_METHOD_BUTTON,
	}
}