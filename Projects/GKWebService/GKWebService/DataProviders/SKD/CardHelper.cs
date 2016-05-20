using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using RubezhClient;

namespace GKWebService.DataProviders.SKD
{
    public static class CardHelper
	{
		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = ClientManager.RubezhService.GetCards(filter);
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetByEmployee(Guid uid)
		{
			var result = ClientManager.RubezhService.GetEmployeeCards(uid);
			return Common.ThrowErrorIfExists(result);
		}

		public static SKDCard GetSingle(Guid uid)
		{
			var result = ClientManager.RubezhService.GetSingleCard(uid);
			return Common.ThrowErrorIfExists(result);
		}

		public static bool Add(SKDCard card, string employeeName)
		{
			var result = ClientManager.RubezhService.AddCard(card, employeeName);
			Common.ThrowErrorIfExists(result);
			return result.Result;
		}

		public static bool Edit(SKDCard card, string employeeName)
		{
			var result = ClientManager.RubezhService.EditCard(card, employeeName);
			Common.ThrowErrorIfExists(result);
			return result.Result;
		}

		public static IEnumerable<SKDCard> GetStopListCards()
		{
			var filter = new CardFilter();
			filter.DeactivationType = LogicalDeletationType.Deleted;
			var result = ClientManager.RubezhService.GetCards(filter);
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetOrganisationCards(Guid organisationUID)
		{
			var filter = new CardFilter();
			filter.EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisationUID }, IsAllPersonTypes = true };
			var result = ClientManager.RubezhService.GetCards(filter);
			return Common.ThrowErrorIfExists(result);
		}

		public static bool DeleteFromEmployee(SKDCard card, string employeeName, string reason)
		{
			var result = ClientManager.RubezhService.DeleteCardFromEmployee(card, employeeName, reason);
			Common.ThrowErrorIfExists(result);
			return result.Result;
		}

		public static bool SaveTemplate(SKDCard card)
		{
			var operationResult = ClientManager.RubezhService.SaveCardTemplate(card);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static bool Delete(SKDCard card)
		{
			var result = ClientManager.RubezhService.DeletedCard(card);
			return Common.ThrowErrorIfExists(result);
		}

		public static DateTime? GetMinDate()
		{
			var result = ClientManager.RubezhService.GetCardsMinDate();
			return Common.ThrowErrorIfExists(result);
		}
	}
}