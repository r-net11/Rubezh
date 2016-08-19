using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StrazhDeviceSDK.API
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

		public bool IsHandicappedCard { get; set; }
	}

	public enum CardType
	{
		[Description("Не известно")]
		NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,

		[Description("Обычная")]
		NET_ACCESSCTLCARD_TYPE_GENERAL,

		[Description("Вип")]
		NET_ACCESSCTLCARD_TYPE_VIP,

		[Description("Гостевая")]
		NET_ACCESSCTLCARD_TYPE_GUEST,

		[Description("Патруль")]
		NET_ACCESSCTLCARD_TYPE_PATROL,

		[Description("Заблокирована")]
		NET_ACCESSCTLCARD_TYPE_BLACKLIST,

		[Description("Принуждение")]
		NET_ACCESSCTLCARD_TYPE_CORCE,

		[Description("Сервисная")]
		NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff
	}

	public enum CardStatus
	{
		[Description("Не известно")]
		NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,

		[Description("Нормальное")]
		NET_ACCESSCTLCARD_STATE_NORMAL = 0,

		[Description("Потеряна")]
		NET_ACCESSCTLCARD_STATE_LOSE = 0x01,

		[Description("Log off")]
		NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,

		[Description("Заморожена")]
		NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,

		[Description("Задолженность")]
		NET_ACCESSCTLCARD_STATE_ARREARAGE = 0x08,

		[Description("Просрочка")]
		NET_ACCESSCTLCARD_STATE_OVERDUE = 0x10,
	}
}