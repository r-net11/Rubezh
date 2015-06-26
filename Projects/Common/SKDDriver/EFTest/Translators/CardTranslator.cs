using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class CardTranslator
	{
		DatabaseContext Context;
		DbService DbService;

		public CardTranslator(DbService dbService)
		{
			Context = dbService.Context;
			DbService = dbService;
		}
        OperationResult<bool> CanSave(API.SKDCard item)
        {
            if (item == null)
                return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
            bool isSameNumber = Context.Cards.Any(x =>
                x.Number == item.Number &&
                x.UID != item.UID);
            if (isSameNumber)
                return OperationResult<bool>.FromError("Попытка добавить карту с повторяющимся номером");
            else
                return new OperationResult<bool>();
        }

		public IQueryable<Card> GetTableItems()
        {
            return Context.Cards.Include(x => x.CardDoors).Include(x => x.Employee).Include(x => x.AccessTemplate).Include(x => x.GKControllerUIDs);
        }

        
        public OperationResult<List<API.SKDCard>> Get(API.CardFilter filter)
		{
			try
			{
                var tableItems = GetTableItems().ToList();
				var result = tableItems.Select(x => Translate(x)).ToList();
				return new OperationResult<List<API.SKDCard>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<API.SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<List<API.SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			try
			{
                var tableItems = GetTableItems().Where(x => x.EmployeeUID == employeeUID).ToList();
				var result = tableItems.Select(x => Translate(x)).ToList();
				return new OperationResult<List<API.SKDCard>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<API.SKDCard>>.FromError(e.Message);
			}
		}

        public OperationResult<List<API.SKDCard>> GetByAccessTemplateUID(Guid accessTemplateUID)
        {
            try
            {
                var tableItems = GetTableItems().Where(x => x.AccessTemplateUID == accessTemplateUID).ToList();
                var result = tableItems.Select(x => Translate(x)).ToList();
                return new OperationResult<List<API.SKDCard>>(result);
            }
            catch (System.Exception e)
            {
                return OperationResult<List<API.SKDCard>>.FromError(e.Message);
            }
        }

		public OperationResult<bool> Save(API.SKDCard item)
		{
			try
			{
                var canSave = CanSave(item);
                if (canSave.HasError)
                    return canSave;
                var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					tableItem = new Card { UID = item.UID };
					TranslateBack(item, tableItem);
					Context.Cards.Add(tableItem);
				}
				else
				{
					TranslateBack(item, tableItem);
				}
				Context.SaveChanges();
				return new OperationResult<bool>(true);
			}
			catch (System.Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult ToStopList(API.SKDCard item, string reason = null)
		{
			try
			{
				var card = Context.Cards.FirstOrDefault(x => x.UID == item.UID);
				if (card != null)
				{
					card.EmployeeUID = null;
					card.StartDate = DateTime.Now;
					card.EndDate = DateTime.Now;
					card.UserTime = 0;
					card.DeactivationControllerUID = Guid.Empty;
					card.CardDoors = new List<CardDoor>();
					card.PassCardTemplateUID = null;
					card.Password = null;
					card.IsInStopList = true;
					card.StopReason = reason;
					card.EmployeeUID = null;
					card.GKLevel = 0;
					card.GKLevelSchedule = 0;
					card.GKLevelSchedule = 0;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Delete(API.SKDCard item)
		{
			try
			{
				var card = Context.Cards.FirstOrDefault(x => x.UID == item.UID);
				if (card != null)
				{
					Context.Cards.Remove(card);
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<API.SKDCard> GetSingle(Guid? uid)
		{
			try
			{
                if (uid == null)
                    return new OperationResult<API.SKDCard>(null);
                var tableItems = GetTableItems()
					.FirstOrDefault(x => x.UID == uid);
				var result = Translate(tableItems);
				return new OperationResult<API.SKDCard>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<API.SKDCard>.FromError(e.Message);
			}
		}

		public OperationResult SavePassTemplate(API.SKDCard card)
		{
			try
			{
				var tableItem = Context.Cards.FirstOrDefault(x => x.UID == card.UID);
				if (tableItem == null)
					return new OperationResult("Карта не найдена в базе данных");
				tableItem.PassCardTemplateUID = card.PassCardTemplateUID;
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public FiresecAPI.SKD.SKDCard Translate(Card tableItem)
		{
			return new FiresecAPI.SKD.SKDCard
			{
				UID = tableItem.UID,
				Number = (uint)tableItem.Number,
				EmployeeUID = tableItem.EmployeeUID,
				StartDate = tableItem.StartDate,
				EndDate = tableItem.EndDate,
				UserTime = tableItem.UserTime,
				DeactivationControllerUID = tableItem.DeactivationControllerUID,
				CardDoors = tableItem.CardDoors.Select(x => x.Translate()).ToList(),
				PassCardTemplateUID = tableItem.PassCardTemplateUID,
				CardType = (FiresecAPI.SKD.CardType)tableItem.CardType,
				GKCardType = (FiresecAPI.GK.GKCardType)tableItem.GKCardType,
				Password = tableItem.Password,
				IsInStopList = tableItem.IsInStopList,
				StopReason = tableItem.StopReason,
				EmployeeName = tableItem.Employee != null ? tableItem.Employee.Name : null,
				OrganisationUID = tableItem.Employee != null ? tableItem.Employee.OrganisationUID.GetValueOrDefault() : Guid.Empty,
				GKLevel = tableItem.GKLevel,
				GKLevelSchedule = tableItem.GKLevelSchedule,
				GKControllerUIDs = tableItem.GKControllerUIDs.Select(x => x.UID).ToList()
			};
		}

		public void TranslateBack(FiresecAPI.SKD.SKDCard apiItem, Card tableItem)
		{
			tableItem.Number = (int)apiItem.Number;
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.StartDate = apiItem.StartDate;
			tableItem.EndDate = apiItem.EndDate;
			tableItem.UserTime = apiItem.UserTime;
			tableItem.DeactivationControllerUID = apiItem.DeactivationControllerUID;
			tableItem.CardDoors = apiItem.CardDoors.Select(x => new CardDoor(x)).ToList();
			tableItem.PassCardTemplateUID = apiItem.PassCardTemplateUID;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.CardType = (int)apiItem.CardType;
			tableItem.GKCardType = (int)apiItem.GKCardType;
			tableItem.Password = apiItem.Password;
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.GKLevel = apiItem.GKLevel;
			tableItem.GKControllerUIDs = apiItem.GKControllerUIDs.Select(x => new CardGKControllerUID
			{
				UID = Guid.NewGuid(),
				CardUID = tableItem.UID,
				GKControllerUID = x
			}).ToList();
		}

		public Card CreateCard(API.SKDCard apiItem)
		{
			var tableItem = new Card { UID = apiItem.UID };
			TranslateBack(apiItem, tableItem);
			return tableItem;
		}

        public OperationResult<Guid> GetEmployeeByCardNo(int cardNo)
        {
            try
            {
                var card = Context.Cards.FirstOrDefault(x => x.Number == cardNo && x.EmployeeUID != null);
                if (card != null)
                {
                    return new OperationResult<Guid>(card.EmployeeUID.Value);
                }
                else
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
                var result = Context.Cards.Min(x => x.EndDate);
                return new OperationResult<DateTime>(result);
            }
            catch (Exception e)
            {
                return OperationResult<DateTime>.FromError(e.Message);
            }
        }

		#region Pending
		public OperationResult AddPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, API.PendingCardAction.Add);
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
					(x.Action == (int)API.PendingCardAction.Add || x.Action == (int)API.PendingCardAction.Edit)))
					return new OperationResult();
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, API.PendingCardAction.Edit);
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
					InsertPendingCard(cardUID, controllerUID, API.PendingCardAction.Delete);
					return new OperationResult();
				}
				DeleteAllPendingCards(cardUID, controllerUID);
				if ((API.PendingCardAction)pendingCard.Action != API.PendingCardAction.Add)
				{
					InsertPendingCard(cardUID, controllerUID, API.PendingCardAction.Delete);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public void DeleteAllPendingCards(Guid? cardUID, Guid controllerUID)
		{
			var pendingCardsToRemove = Context.PendingCards.Where(x => x.CardUID == cardUID && x.ControllerUID == controllerUID);
			Context.PendingCards.RemoveRange(pendingCardsToRemove);
			Context.SaveChanges();
		}

		public void DeleteAllPendingCards(Guid controllerUID)
		{
			var pendingCardsToRemove = Context.PendingCards.Where(x => x.ControllerUID == controllerUID);
			Context.PendingCards.RemoveRange(pendingCardsToRemove);
			Context.SaveChanges();
		}

		void InsertPendingCard(Guid? cardUID, Guid controllerUID, API.PendingCardAction action)
		{
			var pendingCard = new DataClasses.PendingCard
			{
				UID = Guid.NewGuid(),
				CardUID = cardUID,
				Action = (int)action,
				ControllerUID = controllerUID
			};
			Context.PendingCards.Add(pendingCard);
			Context.SaveChanges();
		}

		public IEnumerable<DataClasses.PendingCard> GetAllPendingCards(Guid controllerUID)
		{
			return Context.PendingCards.Where(x => x.ControllerUID == controllerUID);
		}
        #endregion

        
	}
}
