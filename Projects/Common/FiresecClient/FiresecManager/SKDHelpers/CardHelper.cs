using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class CardHelper
	{
		public static IEnumerable<SKDCard> Get(CardFilter filter)
		{
			var result = FiresecManager.FiresecService.GetCards(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<SKDCard> GetByEmployee(Guid uid)
		{
			var result = FiresecManager.FiresecService.GetEmployeeCards(uid);
			return Common.ShowErrorIfExists(result);
		}

		//public static bool ResetRepeatEnter(List<SKDCard> cards, List<Guid> doorsdGuids)
		/// <summary>
		/// Метод сброса антипессбэка для карт
		/// </summary>
		/// <param name="cardsToReset">Словарь карт, подлежащих сбросу. Ключ - карта, Значение - список точек доступа, которые нужно сбросить для конкретной карты</param>
		/// <param name="cardNo">Номер карты</param>
		/// <param name="doorName">Имя точки доступа</param>
		/// <param name="organisationName">Название организации</param>
		/// <returns>true - в случае успешного завершения, false - в случае неудачи</returns>
		public static bool ResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo = null, string doorName = null, string organisationName = null)
		{
			var result = FiresecManager.FiresecService.ResetRepeatEnter(cardsToReset, cardNo, doorName, organisationName);
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

		public static IEnumerable<SKDCard> GetOrganisationCards(Guid organisationUID)
		{
			var filter = new CardFilter();
			filter.EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisationUID }, IsAllPersonTypes = true };
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

		public static bool Delete(SKDCard card)
		{
			var result = FiresecManager.FiresecService.DeletedCard(card);
			return Common.ShowErrorIfExists(result);
		}

		public static DateTime? GetMinDate()
		{
			var result = FiresecManager.FiresecService.GetCardsMinDate();
			return Common.ShowErrorIfExists(result);
		}
	}
}