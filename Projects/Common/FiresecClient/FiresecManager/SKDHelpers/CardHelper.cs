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
		 
		public static bool Add(SKDCard card)
		{
			var result = FiresecManager.FiresecService.AddCard(card);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool Edit(SKDCard card, bool showError = true)
		{
			var result = FiresecManager.FiresecService.EditCard(card);
			Common.ShowErrorIfExists(result, showError);
			return result.Result;
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

		public static bool Delete(Guid uid)
		{
			var result = FiresecManager.FiresecService.DeletedCard(uid);
			return Common.ShowErrorIfExists(result);
		}
	}
}