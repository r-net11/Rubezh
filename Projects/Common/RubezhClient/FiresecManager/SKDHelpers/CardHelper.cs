using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient.SKDHelpers
{
	public static class CardHelper
	{
		public static IEnumerable<SKDCard> Get(CardFilter filter, bool isShowError = true)
		{
			var result = ClientManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result, isShowError);
		}

		public static IEnumerable<SKDCard> GetByEmployee(Guid uid)
		{
			var result = ClientManager.FiresecService.GetEmployeeCards(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static SKDCard GetSingle(Guid uid)
		{
			var result = ClientManager.FiresecService.GetSingleCard(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Add(SKDCard card, string employeeName)
		{
			var result = ClientManager.FiresecService.AddCard(card, employeeName);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool Edit(SKDCard card, string employeeName, bool showError = true)
		{
			var result = ClientManager.FiresecService.EditCard(card, employeeName);
			Common.ShowErrorIfExists(result, showError);
			return result.Result;
		}

		public static IEnumerable<SKDCard> GetStopListCards()
		{
			var filter = new CardFilter();
			filter.DeactivationType = LogicalDeletationType.Deleted;
			var result = ClientManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetOrganisationCards(Guid organisationUID)
		{
			var filter = new CardFilter();
			filter.EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisationUID }, IsAllPersonTypes = true };
			var result = ClientManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool DeleteFromEmployee(SKDCard card, string employeeName, string reason)
		{
			var result = ClientManager.FiresecService.DeleteCardFromEmployee(card, employeeName, reason);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool SaveTemplate(SKDCard card)
		{
			var operationResult = ClientManager.FiresecService.SaveCardTemplate(card);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Delete(SKDCard card)
		{
			var result = ClientManager.FiresecService.DeletedCard(card);
			return Common.ShowErrorIfExists(result);
		}

		public static DateTime? GetMinDate()
		{
			var result = ClientManager.FiresecService.GetCardsMinDate();
			return Common.ShowErrorIfExists(result);
		}
	}
}