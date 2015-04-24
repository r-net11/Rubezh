﻿using System;
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

		protected override SKDCard Translate(DataAccess.Card tableItem)
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
			result.PassCardTemplateUID = tableItem.PassCardTemplateUID;
			result.DeactivationControllerUID = tableItem.DeactivationControllerUID != null ? tableItem.DeactivationControllerUID.Value : Guid.Empty ;
			result.Password = tableItem.Password;
			result.UserTime = tableItem.UserTime;
			result.GKLevel = tableItem.GKLevel;
			result.GKLevelSchedule = tableItem.GKLevelSchedule;
			result.GKCardType = (GKCardType)tableItem.GKCardType;
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
			return result;
		}

		protected override void TranslateBack(DataAccess.Card tableItem, SKDCard apiItem)
		{
			//base.TranslateBack(tableItem, apiItem);
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
			if(filter.EmployeeFilter != null)
			{
				var employeeUIDs = DatabaseService.EmployeeTranslator.GetTableItems(filter.EmployeeFilter).Select(x => x.UID);
				if(filter.DeactivationType == LogicalDeletationType.All)
					result = result.Where(e => e.IsInStopList || (e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value)));
			    else
			        result = result.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
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

		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				var result = Table.Min(x => x.EndDate);
				return new OperationResult<DateTime> { Result = result };
			}
			catch (Exception e)
			{
				return new OperationResult<DateTime>(e.Message);
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

		OperationResult AddPending(Guid cardUID, Guid controllerUID)
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

		OperationResult EditPending(Guid cardUID, Guid controllerUID)
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

		OperationResult DeletePending(Guid cardUID, Guid controllerUID)
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
}