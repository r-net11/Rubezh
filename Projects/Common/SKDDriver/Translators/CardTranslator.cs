using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
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
				return new OperationResult("Попытка добавить карту с повторяющимся номером");
			else
				return new OperationResult();
		}

		//public SKDCard TranslateCard(DataAccess.Card card, DataAccess.Employee employee, IEnumerable<DataAccess.CardDoor> cardDoors)
		//{
		//    return Translate(card, employee, cardDoors);
		//}

		SKDCard Translate(TableCard tableCard)
		{
			var card = tableCard.Card;
			var result = new SKDCard();
			result.UID = card.UID;
			result.HolderUID = card.EmployeeUID;
			result.Number = (uint)card.Number;
			result.CardType = (CardType)card.CardType;
			result.StartDate = card.StartDate;
			result.EndDate = card.EndDate;
			result.AccessTemplateUID = card.AccessTemplateUID;
			result.CardDoors = tableCard.CardDoors.Select(x => DatabaseService.CardDoorTranslator.TranslateCardDoor(x)).ToList();
			result.IsInStopList = card.IsInStopList;
			result.StopReason = card.StopReason;
			result.PassCardTemplateUID = card.PassCardTemplateUID;
			result.DeactivationControllerUID = card.DeactivationControllerUID != null ? card.DeactivationControllerUID.Value : Guid.Empty;
			result.Password = card.Password;
			result.UserTime = card.UserTime;
			result.GKLevel = card.GKLevel;
			result.GKLevelSchedule = card.GKLevelSchedule;
			if (card.GKCardType != -1)
				result.GKCardType = (GKCardType)card.GKCardType;
			if (card.EmployeeUID.HasValue)
			{
				result.EmployeeUID = card.EmployeeUID.Value;
			}
			var employee = tableCard.Employee;
			if (employee != null)
			{
				result.EmployeeName = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
				result.OrganisationUID = employee.OrganisationUID.HasValue ? employee.OrganisationUID.Value : Guid.Empty;
			}
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
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.PassCardTemplateUID = apiItem.PassCardTemplateUID;
			tableItem.Password = apiItem.Password;
			tableItem.DeactivationControllerUID = apiItem.DeactivationControllerUID;
			tableItem.UserTime = apiItem.UserTime;
			tableItem.GKLevel = (byte)apiItem.GKLevel;
			tableItem.GKLevelSchedule = (byte)apiItem.GKLevelSchedule;
			tableItem.GKCardType = (int)apiItem.GKCardType;
			if (tableItem.ExternalKey == null)
				tableItem.ExternalKey = "-1";
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
				if(tableItem == null)
					return new OperationResult("Карта не найдена в базе данных");
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
				if(filter.IsWithInactive)
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
				Context.CommandTimeout = 600;
				var tableItems = GetTableItems(IsInFilter(filter), DatabaseService.EmployeeTranslator.GetFilterExpression((filter.EmployeeFilter)));
				var result = tableItems.Where(x => x.Employee != null || x.Card.IsInStopList).Select(x => Translate(x)).ToList();
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
				var predicate = PredicateBuilder.True<DataAccess.Card>();
				predicate.And(x => x.AccessTemplateUID.HasValue && x.AccessTemplateUID == accessTemplateUID);
				var tableItems = GetTableItems(predicate);
				var skdCards = tableItems.Select(x => Translate(x)).ToList();
				return new OperationResult<List<SKDCard>>(skdCards);
			}
			catch (Exception e)
			{
				return OperationResult<List<SKDCard>>.FromError(e.Message);
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
					return OperationResult<Guid>.FromError("Карта не найдена");
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
				Context.CommandTimeout = 600;
				var predicate = PredicateBuilder.True<DataAccess.Card>();
				predicate.And(x => x.EmployeeUID == employeeUID);
				var tableItems = GetTableItems(predicate);
				var result = tableItems.Where(x => x.Employee != null || x.Card.IsInStopList).Select(x => Translate(x)).ToList();
				return new OperationResult<IEnumerable<SKDCard>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<SKDCard>>.FromError(e.Message);
			}
		}

		public override OperationResult<SKDCard> GetSingle(Guid? uid)
		{
			try
			{
				Context.CommandTimeout = 600;
				var predicate = PredicateBuilder.True<DataAccess.Card>();
				predicate.And(x => x.UID == uid);
				var tableItem = GetTableItems(predicate).FirstOrDefault();
				var result = Translate(tableItem);
				return new OperationResult<SKDCard>(result);
			}
			catch (Exception e)
			{
				return OperationResult<SKDCard>.FromError(e.Message);
			}
		}

		IEnumerable<TableCard> GetTableItems(Expression<Func<DataAccess.Card, bool>> filterExpression, Expression<Func<DataAccess.Employee, bool>> employeeFilterExpression = null)
		{
			return	from card in Context.Cards.Where(filterExpression)
					join cardDoor in Context.CardDoors on card.UID equals cardDoor.CardUID into cardDoors
					join employee in employeeFilterExpression != null ? Context.Employees.Where(employeeFilterExpression) : Context.Employees
						on card.EmployeeUID equals employee.UID into employees
					from employee in employees.DefaultIfEmpty()
					select new TableCard { Card = card, CardDoors = cardDoors, Employee = employee };
		}

		class TableCard
		{
			public DataAccess.Card Card;
			public IEnumerable<DataAccess.CardDoor> CardDoors;
			public DataAccess.Employee Employee;
		}

		#region Pending
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
				DeleteAllPendingCards(cardUID, controllerUID);
				if ((PendingCardAction)pendingCard.Action != PendingCardAction.Add)
				{
					InsertPendingCard(cardUID, controllerUID, PendingCardAction.Delete);
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
			return Context.PendingCards.Where(x => x.ControllerUID == controllerUID);
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
}