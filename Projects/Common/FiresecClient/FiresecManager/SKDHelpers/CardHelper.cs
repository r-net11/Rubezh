using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class CardHelper
	{
		public static void LinkToEmployee(SKDCard card, Guid employeeUid)
		{
			card.HolderUid = employeeUid;
			FiresecManager.FiresecService.SaveCards(new List<SKDCard> { card });
		}

		public static void ToStopList(SKDCard card, string reason)
		{
			card.IsInStopList = true;
			card.StopReason = reason;
			card.HolderUid = null;
			FiresecManager.FiresecService.SaveCards(new List<SKDCard> { card });
			
			var cardZoneLinks = FiresecManager.GetCardZoneLinks(new CardZoneFilter{ Uids =  card.ZoneLinkUids });
			foreach (var item in cardZoneLinks)
				item.CardUid = null;
			FiresecManager.SaveCardZoneLinks(cardZoneLinks);
		}

		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}
	}
}
