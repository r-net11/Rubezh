using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class CardHelper
	{
		public static bool DeleteFromEmployee(SKDCard card, string reason)
		{
			var result = FiresecManager.FiresecService.DeleteCardFromEmployee(card, reason);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetStopListCards()
		{
			var filter = new CardFilter();
			filter.DeactivationType = LogicalDeletationType.Deleted;
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}
		 
		public static bool Add(SKDCard card, bool showError = true)
		{
			var result = FiresecManager.FiresecService.AddCard(card);
			return Common.ShowErrorIfExists(result, showError);
		}

		public static bool Edit(SKDCard card, bool showError = true)
		{
			var result = FiresecManager.FiresecService.EditCard(card);
			return Common.ShowErrorIfExists(result, showError);
		}

		public static bool SaveTemplate(SKDCard card)
		{
			var operationResult = FiresecManager.FiresecService.SaveCardTemplate(card);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var result = FiresecManager.FiresecService.MarkDeletedCard(uid);
			return Common.ShowErrorIfExists(result);
		}
	}
}