using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class CardHelper
	{
		public static bool ToStopList(SKDCard card, string reason)
		{
			card.IsInStopList = true;
			card.StopReason = reason;
			card.HolderUID = null;
			card.CardZones = null;
			var result = FiresecManager.FiresecService.SaveCard(card);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetStopListCards()
		{
			var filter = new CardFilter();
			filter.WithDeleted = DeletedType.Deleted;
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(SKDCard card)
		{
			var result = FiresecManager.FiresecService.SaveCard(card);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveTemplate(SKDCard card)
		{
			var operationResult = FiresecManager.FiresecService.SaveCardTemplate(card);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(SKDCard card)
		{
			return MarkDeleted(card.UID);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var result = FiresecManager.FiresecService.MarkDeletedCard(uid);
			return Common.ShowErrorIfExists(result);
		}
	}
}
