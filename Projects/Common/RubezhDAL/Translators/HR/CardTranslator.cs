using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using SKDAPI = RubezhAPI.SKD;
using GKAPI = RubezhAPI.GK;
using System.Threading;
using System.Diagnostics;

namespace RubezhDAL.DataClasses
{
	public class CardTranslator : ITranslatorGet<Card, SKDAPI.SKDCard, SKDAPI.CardFilter>
	{
		DatabaseContext Context { get { return DbService.Context; } }
		public DbSet<Card> Table { get { return Context.Cards; } }
		public DbService DbService { get; private set; }
		public CardTranslator(DbService dbService)
		{
			DbService = dbService;
		}
		OperationResult<bool> CanSave(SKDAPI.SKDCard item)
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

		public IQueryable<Card> GetFilteredTableItems(SKDAPI.CardFilter filter, IQueryable<Card> tableItems)
		{
			if (filter.EmployeeFilter != null)
			{
				filter.EmployeeFilter.IsAllPersonTypes = true;
				var employees = DbService.EmployeeTranslator.ShortTranslator.GetFilteredTableItems(filter.EmployeeFilter);
				tableItems = tableItems.Where(x => employees.Contains(x.Employee) || x.IsInStopList);
			}
			if (filter.UIDs != null && filter.UIDs.Count > 0)
				tableItems = tableItems.Where(x => filter.UIDs.Contains(x.UID));
			if (filter.DeactivationType == SKDAPI.LogicalDeletationType.Deleted)
				tableItems = tableItems.Where(x => x.IsInStopList);
			if (filter.DeactivationType == SKDAPI.LogicalDeletationType.Active)
				tableItems = tableItems.Where(x => !x.IsInStopList);
			if (filter.IsWithEndDate)
			{
				var endDate = filter.EndDate.Date.AddDays(1);
				tableItems = tableItems.Where(x => x.EndDate < endDate);
			}
			return tableItems.OrderBy(x => x.UID);
		}

		public OperationResult<List<SKDAPI.SKDCard>> Get(SKDAPI.CardFilter filter)
		{
			try
			{
				var tableItems = GetFilteredTableItems(filter, GetTableItems());
				var result = GetAPIItems(tableItems).ToList();
				return new OperationResult<List<SKDAPI.SKDCard>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<SKDAPI.SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<List<SKDAPI.SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			try
			{
				var tableItems = GetTableItems().Where(x => x.EmployeeUID == employeeUID);
				var result = GetAPIItems(tableItems).ToList();
				return new OperationResult<List<SKDAPI.SKDCard>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<SKDAPI.SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<List<SKDAPI.SKDCard>> GetByAccessTemplateUID(Guid accessTemplateUID)
		{
			try
			{
				var tableItems = GetTableItems().Where(x => x.AccessTemplateUID == accessTemplateUID);
				var result = GetAPIItems(tableItems).ToList();
				return new OperationResult<List<SKDAPI.SKDCard>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<SKDAPI.SKDCard>>.FromError(e.Message);
			}
		}

		public OperationResult<bool> Save(SKDAPI.SKDCard item)
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
					Context.CardDoors.RemoveRange(tableItem.CardDoors);
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

		public OperationResult ToStopListByOrganisation(Guid organisationUID, string reason = null)
		{
			try
			{
				var cards = Context.Cards.Include(x => x.Employee).Where(x => x.Employee.OrganisationUID == organisationUID && !x.IsInStopList);
				foreach (var card in cards)
				{
					CreateStopListCard(card, reason);
				}
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult ToStopList(SKDAPI.SKDCard item, string reason = null)
		{
			try
			{
				var card = Context.Cards.FirstOrDefault(x => x.UID == item.UID);
				if (card != null)
				{
					CreateStopListCard(card, reason);
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void CreateStopListCard(Card card, string reason)
		{
			card.EmployeeUID = null;
			card.EndDate = DateTime.Now;
			card.CardDoors = new List<CardDoor>();
			card.IsInStopList = true;
			card.StopReason = reason;
			card.EmployeeUID = null;
			card.GKLevel = 0;
			card.GKLevelSchedule = 0;
			card.GKLevelSchedule = 0;
		}

		public OperationResult Delete(SKDAPI.SKDCard item)
		{
			try
			{
				var card = Context.Cards
					.Include(x => x.CardDoors)
					.Include(x => x.PendingCards)
					.Include(x => x.GKControllerUIDs)
					.FirstOrDefault(x => x.UID == item.UID);
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

		public OperationResult<SKDAPI.SKDCard> GetSingle(Guid? uid)
		{
			try
			{
				if (uid == null)
					return new OperationResult<SKDAPI.SKDCard>(null);
				var tableItems = GetTableItems().Where(x => x.UID == uid);
				var result = GetAPIItems(tableItems).FirstOrDefault();
				return new OperationResult<SKDAPI.SKDCard>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<SKDAPI.SKDCard>.FromError(e.Message);
			}
		}

		public OperationResult SavePassTemplate(SKDAPI.SKDCard card)
		{
			try
			{
				var tableItem = Context.Cards.FirstOrDefault(x => x.UID == card.UID);
				if (tableItem == null)
					return new OperationResult("Карта не найдена в базе данных");
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		IEnumerable<RubezhAPI.SKD.SKDCard> GetAPIItems(IQueryable<Card> tableItems)
		{
			return tableItems.Select(tableItem =>
				new RubezhAPI.SKD.SKDCard
				{
					UID = tableItem.UID,
					NumberInt = tableItem.Number,
					EmployeeUID = tableItem.EmployeeUID,
					EndDate = tableItem.EndDate,
					CardDoors = tableItem.CardDoors.Select(x => new RubezhAPI.SKD.CardDoor
						{
							UID = x.UID,
							CardUID = x.CardUID,
							DoorUID = x.DoorUID,
							AccessTemplateUID = x.AccessTemplateUID,
							EnterScheduleNo = x.EnterScheduleNo,
							ExitScheduleNo = x.ExitScheduleNo
						}).ToList(),
					AccessTemplateUID = tableItem.AccessTemplateUID,
					GKCardType = (RubezhAPI.GK.GKCardType)tableItem.GKCardType,
					IsInStopList = tableItem.IsInStopList,
					StopReason = tableItem.StopReason,
					EmployeeName = tableItem.Employee != null ? tableItem.Employee.LastName + " " + tableItem.Employee.FirstName + " " + tableItem.Employee.SecondName : null,
					OrganisationUID = tableItem.Employee != null ? tableItem.Employee.OrganisationUID != null ? tableItem.Employee.OrganisationUID.Value : Guid.Empty : Guid.Empty,
					GKLevel = tableItem.GKLevel,
					GKLevelSchedule = tableItem.GKLevelSchedule,
					GKControllerUIDs = tableItem.GKControllerUIDs.Select(x => x.GKControllerUID).ToList()
				});
		}

		public void TranslateBack(RubezhAPI.SKD.SKDCard apiItem, Card tableItem)
		{
			tableItem.Number = (int)apiItem.Number;
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.EndDate = apiItem.EndDate;
			tableItem.CardDoors = apiItem.CardDoors.Select(x => new CardDoor(x)).ToList();
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.GKCardType = (int)apiItem.GKCardType;
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.GKLevel = apiItem.GKLevel;
			tableItem.GKLevelSchedule = apiItem.GKLevelSchedule;
			tableItem.GKControllerUIDs = apiItem.GKControllerUIDs.Select(x => new CardGKControllerUID
			{
				UID = Guid.NewGuid(),
				CardUID = tableItem.UID,
				GKControllerUID = x
			}).ToList();
		}

		public Card CreateCard(SKDAPI.SKDCard apiItem)
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

		public OperationResult<List<SKDAPI.CardAccessTemplateDoors>> GetAccessTemplateDoorsByOrganisation(Guid organisationUID)
		{
			try
			{
				var result = Context.Cards
					.Include(x => x.Employee)
					.Include(x => x.AccessTemplate)
					.Include(x => x.AccessTemplate.CardDoors)
					.Select(tableItem => new SKDAPI.CardAccessTemplateDoors
						{
							CardUID = tableItem.UID,
							CardDoors = tableItem.AccessTemplate.CardDoors.Select(x => new RubezhAPI.SKD.CardDoor
								{
									UID = x.UID,
									CardUID = x.CardUID,
									DoorUID = x.DoorUID,
									AccessTemplateUID = x.AccessTemplateUID,
									EnterScheduleNo = x.EnterScheduleNo,
									ExitScheduleNo = x.ExitScheduleNo
								}).ToList()
						}).ToList();
				return new OperationResult<List<SKDAPI.CardAccessTemplateDoors>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<SKDAPI.CardAccessTemplateDoors>>.FromError(e.Message);
			}
		}

		#region Pending
		public OperationResult AddPending(Guid cardUID, Guid controllerUID)
		{
			try
			{
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Add);
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
					(x.Action == (int)SKDAPI.PendingCardAction.Add || x.Action == (int)SKDAPI.PendingCardAction.Edit)))
					return new OperationResult();
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Edit);
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
					InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Delete);
					return new OperationResult();
				}
				DeleteAllPendingCards(cardUID, controllerUID);
				if ((SKDAPI.PendingCardAction)pendingCard.Action != SKDAPI.PendingCardAction.Add)
				{
					InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Delete);
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

		void InsertPendingCard(Guid? cardUID, Guid controllerUID, SKDAPI.PendingCardAction action)
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

		public List<DataClasses.PendingCard> GetAllPendingCards(Guid controllerUID)
		{
			try
			{
				return Context.PendingCards.Where(x => x.ControllerUID == controllerUID).ToList();
			}
			catch (Exception)
			{
				return null;
			}
			
		}
		#endregion

		public OperationResult RemoveAccessTemplate(List<Guid> uids)
		{
			try
			{
				var cards = Context.Cards.Where(x => uids.Any(y => y == x.UID));
				foreach (var card in cards)
				{
					card.AccessTemplateUID = null;
				}
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<List<GKAPI.GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> deviceDoorUIDs)
		{
			try
			{
				var tableUsers = Table
					.Include(x => x.Employee)
					.Include(x => x.GKControllerUIDs)
					.Include(x => x.CardDoors)
					.Include(x => x.AccessTemplate.CardDoors)
					.Where(x => !x.IsInStopList)
					.Select(x => new
						{
							ExpirationDate = x.EndDate,
							FirstName = x.Employee.FirstName,
							SecondName = x.Employee.SecondName,
							LastName = x.Employee.LastName,
							GkLevel = x.GKLevel,
							GkLevelSchedule = x.GKLevelSchedule,
							UserType = x.GKCardType,
							Password = x.Number,
							CardDoors = x.CardDoors,
							GKControllerUIDs = x.GKControllerUIDs
						})
					.OrderBy(x => x.Password)
					.ToList();
				var filteredTableUsers = tableUsers
					.Where(x => x.GKControllerUIDs.Any(gkControllerUID => gkControllerUID.GKControllerUID == deviceUID)
						|| x.CardDoors.Any(cardDoor => deviceDoorUIDs.Any( doorUID => doorUID == cardDoor.DoorUID)));
				var users = new List<GKAPI.GKUser>();
				foreach (var tableUser in filteredTableUsers)
				{
					var user = new GKAPI.GKUser
					{
						ExpirationDate = tableUser.ExpirationDate,
						Fio = tableUser.LastName + " " + tableUser.FirstName + " " + tableUser.SecondName,
						GkLevel = (byte)tableUser.GkLevel,
						GkLevelSchedule = (byte)tableUser.GkLevelSchedule,
						Password = (uint)tableUser.Password,
						UserType = (GKAPI.GKCardType)tableUser.UserType
					};
					foreach (var cardDoor in tableUser.CardDoors)
					{
						var door = GKManager.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
						user.Descriptors.Add(
							new GKAPI.GKUserDescriptor
							{ 
								GKDoor = door, 
								DescriptorNo = door.EnterDevice.GKDescriptorNo, 
								ScheduleNo = cardDoor.EnterScheduleNo
							});
						if(door.DoorType != GKAPI.GKDoorType.OneWay && door.ExitDevice != null)
							user.Descriptors.Add(
								new GKAPI.GKUserDescriptor
								{ 
									GKDoor = door,
									DescriptorNo = door.ExitDevice.GKDescriptorNo,
									ScheduleNo = cardDoor.ExitScheduleNo
								});
					}
					users.Add(user);
				}
				return new OperationResult<List<GKAPI.GKUser>>(users);
			}
			catch(Exception e)
			{
				return OperationResult<List<GKAPI.GKUser>>.FromError(e.Message);
			}
		}
	}
}