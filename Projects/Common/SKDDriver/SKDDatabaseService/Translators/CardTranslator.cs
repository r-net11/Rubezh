using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class CardTranslator : IsDeletedTranslator<DataAccess.Card, SKDCard, CardFilter>
	{
		public CardTranslator(DataAccess.SKDDataContext context, CardDoorTranslator cardDoorTranslator)
			: base(context)
		{
			CardDoorTranslator = cardDoorTranslator;
		}

		CardDoorTranslator CardDoorTranslator;

		protected override OperationResult CanSave(SKDCard item)
		{
			bool isSameNumber = Table.Any(x => x.Number == item.Number &&
				!x.IsDeleted &&
				x.UID != item.UID);
			if (isSameNumber)
				return new OperationResult("Попытка добавить карту с повторяющимся номером");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			var operationResult = GetSingle(uid);
			var card = operationResult.Result;
			if (card != null)
			{
				if (Context.Employees.Any(x => x.UID == card.HolderUID &&
					!x.IsDeleted))
					return new OperationResult("Невозможно удалить карту, пока она указана у действующих сотрудников");
			}
			return base.CanDelete(uid);
		}

		protected override SKDCard Translate(DataAccess.Card tableItem)
		{
			var result = base.Translate(tableItem);
			result.HolderUID = tableItem.EmployeeUID;
			result.Number = tableItem.Number;
			result.CardType = (CardType)tableItem.CardType;
			result.StartDate = tableItem.StartDate;
			result.EndDate = tableItem.EndDate;
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			result.CardDoors = CardDoorTranslator.GetForCards(tableItem.UID);
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
			result.PassCardTemplateUID = tableItem.PassCardTemplateUID;
			result.DeactivationControllerUID = tableItem.DeactivationControllerUID != null ? tableItem.DeactivationControllerUID.Value : Guid.Empty ;
			result.Password = tableItem.Password;

			var employee = Context.Employees.FirstOrDefault(x => x.UID == tableItem.EmployeeUID);
			if (employee != null)
				result.EmployeeName = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
			return result;
		}

		protected override void TranslateBack(DataAccess.Card tableItem, SKDCard apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Number = apiItem.Number;
			tableItem.EmployeeUID = apiItem.HolderUID;
			tableItem.CardType = (int)apiItem.CardType;
			tableItem.StartDate = CheckDate(apiItem.StartDate);
			tableItem.EndDate = CheckDate(apiItem.EndDate);
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.PassCardTemplateUID = apiItem.PassCardTemplateUID;
			tableItem.Password = apiItem.Password;
			tableItem.DeactivationControllerUID = tableItem.DeactivationControllerUID;
		}

		public override OperationResult Save(SKDCard card)
		{
			var updateCardDoorsResult = CardDoorTranslator.RemoveFromCard(card);
			var result = base.Save(card);
			CardDoorTranslator.Save(card.CardDoors);
			return result;
		}
		
		public OperationResult SavePassTemplate(SKDCard card)
		{
			try
			{
				var oprationResult = GetSingle(card.UID);
				if (oprationResult != null)
				{
					oprationResult.Result.PassCardTemplateUID = card.PassCardTemplateUID;
					Context.SubmitChanges();
					return new OperationResult();
				}
				else
				{
					return new OperationResult("Карта не найдена в базе данных");
				}
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
			return result;
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
				return new OperationResult<List<SKDCard>>() { Result = skdCards };
			}
			catch (Exception e)
			{
				return new OperationResult<List<SKDCard>>(e.Message);
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
						return new OperationResult<Guid>() { Result = card.EmployeeUID.Value };
				}
				{
					return new OperationResult<Guid>("Карта не найдена");
				}
			}
			catch (Exception e)
			{
				return new OperationResult<Guid>(e.Message);
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

		public OperationResult AddPending(Guid cardUID, Guid controllerUID)
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

		public OperationResult EditPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				if (Context.PendingCards.Any(x => x.CardUID == cardUID && x.ControllerUID == controllerUID &&
					(x.Action == (int)PendingCardAction.Add || x.Action == (int)PendingCardAction.Edit)))
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

		public OperationResult DeletePending(Guid cardUID, Guid controllerUID)
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

		public IEnumerable<SKDDriver.DataAccess.PendingCard> GetAllPendingCards(Guid controllerUID)
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

		void InsertPendingCard(Guid cardUID, Guid controllerUID, PendingCardAction action)
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
		#endregion
	}

	public enum PendingCardAction
	{
		Add,
		Edit,
		Delete
	}
}