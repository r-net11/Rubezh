using System;
using System.Collections.Generic;
using System.ComponentModel;
using Localization;

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

		public bool IsHandicappedCard { get; set; }
	}

	public enum CardType
	{
		//[Description("Не известно")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_UNKNOWN")]
		NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,

        //[Description("Обычная")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_GENERAL")]
		NET_ACCESSCTLCARD_TYPE_GENERAL,

        //[Description("Вип")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_VIP")]
		NET_ACCESSCTLCARD_TYPE_VIP,

        //[Description("Гостевая")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_GUEST")]
		NET_ACCESSCTLCARD_TYPE_GUEST,

        //[Description("Патруль")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_PATROL")]
		NET_ACCESSCTLCARD_TYPE_PATROL,

        //[Description("Заблокирована")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_BLACKLIST")]
		NET_ACCESSCTLCARD_TYPE_BLACKLIST,

        //[Description("Принуждение")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_CORCE")]
		NET_ACCESSCTLCARD_TYPE_CORCE,

        //[Description("Сервисная")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_TYPE_MOTHERCARD")]
		NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff
	}

	public enum CardStatus
	{
        //[Description("Не известно")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_UNKNOWN")]
		NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,

        //[Description("Нормальное")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_NORMAL")]
		NET_ACCESSCTLCARD_STATE_NORMAL = 0,

        //[Description("Потеряна")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_LOSE")]
		NET_ACCESSCTLCARD_STATE_LOSE = 0x01,

        //[Description("Log off")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_LOGOFF")]
		NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,

        //[Description("Заморожена")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_FREEZE")]
		NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,

        //[Description("Задолженность")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_ARREARAGE")]
		NET_ACCESSCTLCARD_STATE_ARREARAGE = 0x08,

        //[Description("Просрочка")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.Card), "NET_ACCESSCTLCARD_STATE_OVERDUE")]
		NET_ACCESSCTLCARD_STATE_OVERDUE = 0x10,
	}
}