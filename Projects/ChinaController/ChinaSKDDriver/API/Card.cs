using System;
using System.Collections.Generic;

namespace ChinaSKDDriverAPI
{
	public class Card
	{
		public Card()
		{
			Doors = new List<int>();
			TimeSections = new List<int>();
		}

		public int RecordNo { get; set; }
		public string CardNo { get; set; }
		public CardType CardType { get; set; }
		public CardStatus CardStatus { get; set; }
		public string Password { get; set; }
		public int DoorsCount { get; set; }
		public List<int> Doors { get; set; }
		public int TimeSectionsCount { get; set; }
		public List<int> TimeSections { get; set; }
		public int UserTime { get; set; }
		public DateTime ValidStartDateTime { get; set; }
		public DateTime ValidEndDateTime { get; set; }
	}

	public enum CardType
	{
		NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,
		NET_ACCESSCTLCARD_TYPE_GENERAL,
		NET_ACCESSCTLCARD_TYPE_VIP,
		NET_ACCESSCTLCARD_TYPE_GUEST,
		NET_ACCESSCTLCARD_TYPE_PATROL,
		NET_ACCESSCTLCARD_TYPE_BLACKLIST,
		NET_ACCESSCTLCARD_TYPE_CORCE,
		NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff,
	}

	public enum CardStatus
	{
		NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,
		NET_ACCESSCTLCARD_STATE_NORMAL = 0,
		NET_ACCESSCTLCARD_STATE_LOSE = 0x01,
		NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,
		NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,
	}
}