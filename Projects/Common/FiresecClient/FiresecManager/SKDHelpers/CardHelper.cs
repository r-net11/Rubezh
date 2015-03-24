﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class CardHelper
	{
		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static SKDCard GetSingle(Guid uid)
		{
			var result = FiresecManager.FiresecService.GetCards(new CardFilter { UIDs = new List<Guid> { uid }, LogicalDeletationType = LogicalDeletationType.All });
			return Common.ShowErrorIfExists(result).FirstOrDefault();
		}

		public static bool Add(SKDCard card, string employeeName)
		{
			var result = FiresecManager.FiresecService.AddCard(card, employeeName);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool Edit(SKDCard card, string employeeName, bool showError = true)
		{
			var result = FiresecManager.FiresecService.EditCard(card, employeeName);
			Common.ShowErrorIfExists(result, showError);
			return result.Result;
		}

		public static IEnumerable<SKDCard> GetStopListCards()
		{
			var filter = new CardFilter();
			filter.DeactivationType = LogicalDeletationType.Deleted;
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool DeleteFromEmployee(SKDCard card, string employeeName, string reason)
		{
			var result = FiresecManager.FiresecService.DeleteCardFromEmployee(card, employeeName, reason);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool SaveTemplate(SKDCard card)
		{
			var operationResult = FiresecManager.FiresecService.SaveCardTemplate(card);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Delete(Guid uid)
		{
			var result = FiresecManager.FiresecService.DeletedCard(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static DateTime? GetMinDate()
		{
			var result = FiresecManager.FiresecService.GetCardsMinDate();
			return Common.ShowErrorIfExists(result);
		}
	}
}