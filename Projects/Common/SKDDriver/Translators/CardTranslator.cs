using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StrazhDAL.DataAccess;

namespace StrazhDAL
{
	public class CardTranslator : WithFilterTranslator<DataAccess.Card, SKDCard, CardFilter>
	{
		public CardTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override OperationResult CanSave(SKDCard item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool isSameNumber = Table.Any(x =>
				x.Number == item.Number &&
				x.UID != item.UID);
			if (isSameNumber)
                return new OperationResult(Resources.Language.Translators.CardTranslator.CanSave_Error);
			else
				return new OperationResult();
		}

		public SKDCard TranslateCard(DataAccess.Card card, DataAccess.Employee employee, IEnumerable<DataAccess.CardDoor> cardDoors)
		{
			return Translate(card, employee, cardDoors);
		}

		protected override SKDCard Translate(Card tableItem)
		{
			var result = base.Translate(tableItem);
			result.HolderUID = tableItem.EmployeeUID;
			result.Number = (uint)tableItem.Number;
			result.CardType = (CardType)tableItem.CardType;
			result.StartDate = tableItem.StartDate;
			result.EndDate = tableItem.EndDate;
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			result.CardDoors = DatabaseService.CardDoorTranslator.GetForCards(tableItem.UID);
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
			result.IsHandicappedCard = tableItem.IsHandicappedCard;
			result.PassCardTemplateUID = tableItem.PassCardTemplateUID;
			result.DeactivationControllerUID = tableItem.DeactivationControllerUID != null ? tableItem.DeactivationControllerUID.Value : Guid.Empty;
			result.Password = tableItem.Password;
			result.UserTime = tableItem.UserTime;
			if (tableItem.EmployeeUID.HasValue)
			{
				result.EmployeeUID = tableItem.EmployeeUID.Value;
			}

			var employee = Context.Employees.FirstOrDefault(x => x.UID == tableItem.EmployeeUID);
			if (employee != null)
			{
				result.EmployeeName = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
				result.OrganisationUID = employee.OrganisationUID.HasValue ? employee.OrganisationUID.Value : Guid.Empty;
			}

			result.AllowedPassCount = tableItem.AllowedPassCount;
			return result;
		}

		private SKDCard Translate(DataAccess.Card card, DataAccess.Employee employee, IEnumerable<DataAccess.CardDoor> cardDoors)
		{
			var result = new SKDCard();
			result.UID = card.UID;
			result.HolderUID = card.EmployeeUID;
			result.Number = (uint)card.Number;
			result.CardType = (CardType)card.CardType;
			result.StartDate = card.StartDate;
			result.EndDate = card.EndDate;
			result.AccessTemplateUID = card.AccessTemplateUID;
			result.CardDoors = cardDoors.Select(x => DatabaseService.CardDoorTranslator.TranslateCardDoor(x)).ToList();
			result.IsInStopList = card.IsInStopList;
			result.StopReason = card.StopReason;
			result.IsHandicappedCard = card.IsHandicappedCard;
			result.PassCardTemplateUID = card.PassCardTemplateUID;
			result.DeactivationControllerUID = card.DeactivationControllerUID != null ? card.DeactivationControllerUID.Value : Guid.Empty;
			result.Password = card.Password;
			result.UserTime = card.UserTime;
			if (card.EmployeeUID.HasValue)
			{
				result.EmployeeUID = card.EmployeeUID.Value;
			}
			if (employee != null)
			{
				result.EmployeeName = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
				result.OrganisationUID = employee.OrganisationUID.HasValue ? employee.OrganisationUID.Value : Guid.Empty;
			}
			result.AllowedPassCount = card.AllowedPassCount;
			return result;
		}

		protected override void TranslateBack(DataAccess.Card tableItem, SKDCard apiItem)
		{
			tableItem.Number = (int)apiItem.Number;
			tableItem.EmployeeUID = apiItem.HolderUID;
			tableItem.CardType = (int)apiItem.CardType;
			tableItem.StartDate = TranslatiorHelper.CheckDate(apiItem.StartDate);
			tableItem.EndDate = TranslatiorHelper.CheckDate(apiItem.EndDate);
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.IsHandicappedCard = apiItem.IsHandicappedCard;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.PassCardTemplateUID = apiItem.PassCardTemplateUID;
			tableItem.Password = apiItem.Password;
			tableItem.DeactivationControllerUID = apiItem.DeactivationControllerUID;
			tableItem.UserTime = apiItem.UserTime;
			if (tableItem.ExternalKey == null)
				tableItem.ExternalKey = "-1";
			tableItem.AllowedPassCount = apiItem.AllowedPassCount;
		}

		public override OperationResult Save(SKDCard card)
		{
			var updateCardDoorsResult = DatabaseService.CardDoorTranslator.RemoveFromCard(card);
			var result = base.Save(card);
			DatabaseService.CardDoorTranslator.Save(card.CardDoors);
			return result;
		}

		public OperationResult SavePassTemplate(SKDCard card)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == card.UID);
				if (tableItem == null)
                    return new OperationResult(Resources.Language.Translators.CardTranslator.SavePassTemplate_Error);
				tableItem.PassCardTemplateUID = card.PassCardTemplateUID;
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.Card, bool>> IsInFilter(CardFilter filter)
		{
			var result = base.IsInFilter(filter);

			switch (filter.DeactivationType)
			{
				case LogicalDeletationType.Deleted:
					result = result.And(e => e.IsInStopList);
					break;

				case LogicalDeletationType.Active:
					result = result.And(e => !e.IsInStopList);
					break;
			}

			if (filter.IsWithEndDate)
			{
				result = result.And(e => e.EndDate <= filter.EndDate);
			}

			if (filter.CardTypes.IsNotNullOrEmpty())
			{
				if (filter.IsWithInactive)
					result = result.And(e => (e.CardType != null && filter.CardTypes.Contains((CardType)e.CardType.Value)) || e.IsInStopList);
				else
					result = result.And(e => e.CardType != null && filter.CardTypes.Contains((CardType)e.CardType.Value));
			}

			return result;
		}

		public override IEnumerable<DataAccess.Card> GetTableItems(CardFilter filter)
		{
			var result = base.GetTableItems(filter);
			if (filter.EmployeeFilter != null)
			{
				var employeeUIDs = DatabaseService.EmployeeTranslator.GetTableItems(filter.EmployeeFilter).Select(x => x.UID);
				if (filter.DeactivationType == LogicalDeletationType.All)
					result = result.Where(e => e.IsInStopList || (e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value)));
				else
					result = result.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
			}
			return result;
		}

		public override OperationResult<IEnumerable<SKDCard>> Get(CardFilter filter)
		{
			try
			{
				var result = new List<SKDCard>();

				Context.CommandTimeout = 600;
				var tableItems =
					from card in Context.Cards.Where(IsInFilter(filter))
					join cardDoor in Context.CardDoors on card.UID equals cardDoor.CardUID into cardDoors
					join employee in filter.EmployeeFilter != null ? Context.Employees.Where(DatabaseService.EmployeeTranslator.GetFilterExpression((filter.EmployeeFilter))) : Context.Employees
						on card.EmployeeUID equals employee.UID into employees
					from employee in employees.DefaultIfEmpty()
					select new { Card = card, CardDoors = cardDoors, Employee = employee };
				foreach (var tableItem in tableItems.Where(x => x.Employee != null || x.Card.IsInStopList))
					result.Add(Translate(tableItem.Card, tableItem.Employee, tableItem.CardDoors));
				return new OperationResult<IEnumerable<SKDCard>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<List<SKDCard>> GetByAccessTemplateUID(Guid accessTemplateUID)
		{
			try
			{
				var skdCards = new List<SKDCard>();
				var cards = Table.Where(x => x.AccessTemplateUID.HasValue && x.AccessTemplateUID == accessTemplateUID);
				if (cards != null)
				{
					foreach (var card in cards)
					{
						var skdCard = Translate(card);
						skdCards.Add(skdCard);
					}
				}
				return new OperationResult<List<SKDCard>>(skdCards);
			}
			catch (Exception e)
			{
				return OperationResult<List<SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<SKDCard> Get(int cardNo)
		{
			try
			{
				var card = Table.FirstOrDefault(x => x.Number == cardNo);
				if (card == null)
                    return OperationResult<SKDCard>.FromError(Resources.Language.Translators.CardTranslator.Get_Error);

				return new OperationResult<SKDCard>(Translate(card));
			}
			catch (Exception e)
			{
				return OperationResult<SKDCard>.FromError(e.Message);
			}
		}

		public virtual OperationResult<Guid> GetEmployeeByCardNo(int cardNo)
		{
			try
			{
				var cards = Table.Where(x => x.Number == cardNo);
				var card = cards.FirstOrDefault();
				if (card != null)
				{
					if (card.EmployeeUID != null)
						return new OperationResult<Guid>(card.EmployeeUID.Value);
				}
				{
                    return OperationResult<Guid>.FromError(Resources.Language.Translators.CardTranslator.Get_Error);
				}
			}
			catch (Exception e)
			{
				return OperationResult<Guid>.FromError(e.Message);
			}
		}

		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				var result = Table.Min(x => x.EndDate);
				return new OperationResult<DateTime>(result);
			}
			catch (Exception e)
			{
				return OperationResult<DateTime>.FromError(e.Message);
			}
		}

		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			try
			{
				var result = new List<SKDCard>();
				Context.CommandTimeout = 600;
				var tableItems =
					from card in Context.Cards.Where(x => x.EmployeeUID == employeeUID)
					join cardDoor in Context.CardDoors on card.UID equals cardDoor.CardUID into cardDoors
					join employee in Context.Employees on card.EmployeeUID equals employee.UID into employees
					from employee in employees.DefaultIfEmpty()
					select new { Card = card, CardDoors = cardDoors, Employee = employee };
				foreach (var tableItem in tableItems.Where(x => x.Employee != null || x.Card.IsInStopList))
					result.Add(Translate(tableItem.Card, tableItem.Employee, tableItem.CardDoors));
				return new OperationResult<IEnumerable<SKDCard>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<SKDCard>>.FromError(e.Message);
			}
		}

		#region Pending

		public OperationResult AddPendingList(Guid cardUID, IEnumerable<Guid> controllerUIDs)
		{
			foreach (var controllerUID in controllerUIDs)
			{
				var result = AddPending(cardUID, controllerUID);
				if (result.HasError)
					return result;
			}
			return new OperationResult();
		}

		public OperationResult EditPendingList(Guid cardUID, IEnumerable<Guid> controllerUIDs)
		{
			foreach (var controllerUID in controllerUIDs)
			{
				var result = EditPending(cardUID, controllerUID);
				if (result.HasError)
					return result;
			}
			return new OperationResult();
		}

		public OperationResult DeletePendingList(Guid cardUID, IEnumerable<Guid> controllerUIDs)
		{
			foreach (var controllerUID in controllerUIDs)
			{
				var result = DeletePending(cardUID, controllerUID);
				if (result.HasError)
					return result;
			}
			return new OperationResult();
		}

		public OperationResult ResetRepeatEnterPendingList(Guid cardUID, IEnumerable<Guid> controllerUIDs)
		{
			foreach (var controllerUID in controllerUIDs)
			{
				var result = ResetRepeatEnterPending(cardUID, controllerUID);
				if (result.HasError)
					return result;
			}
			return new OperationResult();
		}

		private OperationResult AddPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, PendingCardAction.Add);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		private OperationResult EditPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				if (Context.PendingCards
					.Any(x => x.CardUID == cardUID && x.ControllerUID == controllerUID
						&& (x.Action == (int)PendingCardAction.Add || x.Action == (int)PendingCardAction.Edit)))
					return new OperationResult();
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, PendingCardAction.Edit);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		private OperationResult DeletePending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				var pendingCard = Context.PendingCards.FirstOrDefault(x => x.CardUID == cardUID && x.ControllerUID == controllerUID);
				if (pendingCard == null)
				{
					InsertPendingCard(cardUID, controllerUID, PendingCardAction.Delete);
					return new OperationResult();
				}
				switch ((PendingCardAction)pendingCard.Action)
				{
					case PendingCardAction.Add:
						DeleteAllPendingCards(cardUID, controllerUID);
						break;

					case PendingCardAction.Delete:
					case PendingCardAction.Edit:
						DeleteAllPendingCards(cardUID, controllerUID);
						InsertPendingCard(cardUID, controllerUID, PendingCardAction.Delete);
						break;
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		private OperationResult ResetRepeatEnterPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				PendingCard pendingCard = Context.PendingCards.FirstOrDefault(x => x.CardUID == cardUID && x.ControllerUID == controllerUID);
				if (pendingCard == null)
				{
					InsertPendingCard(cardUID, controllerUID, PendingCardAction.ResetRepeatEnter);
					return new OperationResult();
				}

				pendingCard.Action = (int)PendingCardAction.ResetRepeatEnter;
				try
				{
					Context.SubmitChanges();
				}
				catch (Exception e)
				{
					return new OperationResult(e.Message);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public IEnumerable<PendingCard> GetAllPendingCards(Guid controllerUID)
		{
			var pendingCards = Context.PendingCards.Where(x => x.ControllerUID == controllerUID);
			return pendingCards;
		}

		public void DeleteAllPendingCards(Guid cardUID, Guid controllerUID)
		{
			var pendingCardsToRemove = Context.PendingCards.Where(x => x.CardUID == cardUID && x.ControllerUID == controllerUID);
			Context.PendingCards.DeleteAllOnSubmit(pendingCardsToRemove);
			Context.SubmitChanges();
		}

		public void DeleteAllPendingCards(Guid controllerUID)
		{
			var pendingCardsToRemove = Context.PendingCards.Where(x => x.ControllerUID == controllerUID);
			Context.PendingCards.DeleteAllOnSubmit(pendingCardsToRemove);
			Context.SubmitChanges();
		}

		private void InsertPendingCard(Guid cardUID, Guid controllerUID, PendingCardAction action)
		{
			var pendingCard = new DataAccess.PendingCard
			{
				UID = Guid.NewGuid(),
				CardUID = cardUID,
				Action = (int)action,
				ControllerUID = controllerUID
			};
			Context.PendingCards.InsertOnSubmit(pendingCard);
			Context.SubmitChanges();
		}

		#endregion Pending

		public int GetCardsCount()
		{
			return Context.Cards.Count(x => !x.IsInStopList);
		}
	}
}