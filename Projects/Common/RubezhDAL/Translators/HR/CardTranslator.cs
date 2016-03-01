using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GKAPI = RubezhAPI.GK;
using SKDAPI = RubezhAPI.SKD;

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
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetFilteredTableItems(filter, GetTableItems());
				return GetAPIItems(tableItems).ToList();
			});
		}

		public OperationResult<List<SKDAPI.SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetTableItems().Where(x => x.EmployeeUID == employeeUID);
				return GetAPIItems(tableItems).ToList();
			});
		}

		public OperationResult<List<SKDAPI.SKDCard>> GetByAccessTemplateUID(Guid accessTemplateUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetTableItems().Where(x => x.AccessTemplateUID == accessTemplateUID);
				return GetAPIItems(tableItems).ToList();
			});
		}

		public OperationResult<bool> Save(SKDAPI.SKDCard item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var canSave = CanSave(item);
				if (canSave.HasError)
					throw new Exception(canSave.Error);
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
				return true;
			});
		}

		public OperationResult<bool> ToStopListByOrganisation(Guid organisationUID, string reason = null)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var cards = Context.Cards.Include(x => x.Employee).Where(x => x.Employee.OrganisationUID == organisationUID && !x.IsInStopList);
				foreach (var card in cards)
				{
					CreateStopListCard(card, reason);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> ToStopList(SKDAPI.SKDCard item, string reason = null)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var card = Context.Cards.FirstOrDefault(x => x.UID == item.UID);
				if (card != null)
				{
					CreateStopListCard(card, reason);
					Context.SaveChanges();
				};
				return true;
			});
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

		public OperationResult<bool> Delete(SKDAPI.SKDCard item)
		{
			return DbServiceHelper.InTryCatch(() =>
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
				};
				return true;
			});
		}

		public OperationResult<SKDAPI.SKDCard> GetSingle(Guid? uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (uid == null)
					return null;
				var tableItems = GetTableItems().Where(x => x.UID == uid);
				return GetAPIItems(tableItems).FirstOrDefault();
			});
		}

		public OperationResult<bool> SavePassTemplate(SKDAPI.SKDCard card)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.Cards.FirstOrDefault(x => x.UID == card.UID);
				if (tableItem == null)
					throw new Exception("Карта не найдена в базе данных");
				Context.SaveChanges();
				return true;
			});
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
			return DbServiceHelper.InTryCatch(() =>
			{
				var card = Context.Cards.FirstOrDefault(x => x.Number == cardNo && x.EmployeeUID != null);
				if (card != null)
					return card.EmployeeUID.Value;
				else
					throw new Exception("Карта не найдена");
			});
		}

		public OperationResult<DateTime> GetMinDate()
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				return Context.Cards.Min(x => x.EndDate);
			});
		}

		public OperationResult<List<SKDAPI.CardAccessTemplateDoors>> GetAccessTemplateDoorsByOrganisation(Guid organisationUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				return Context.Cards
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
			});
		}

		#region Pending
		public OperationResult<bool> AddPending(Guid cardUID, Guid controllerUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Add);
				return true;
			});
		}

		public OperationResult<bool> EditPending(Guid cardUID, Guid controllerUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (Context.PendingCards.Any(x => x.CardUID == cardUID && x.ControllerUID == controllerUID &&
					(x.Action == (int)SKDAPI.PendingCardAction.Add || x.Action == (int)SKDAPI.PendingCardAction.Edit)))
					return true;
				DeleteAllPendingCards(cardUID, controllerUID);
				InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Edit);
				return true;
			});
		}

		public OperationResult<bool> DeletePending(Guid cardUID, Guid controllerUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var pendingCard = Context.PendingCards.FirstOrDefault(x => x.CardUID == cardUID && x.ControllerUID == controllerUID);
				if (pendingCard == null)
				{
					InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Delete);
					return true;
				}
				DeleteAllPendingCards(cardUID, controllerUID);
				if ((SKDAPI.PendingCardAction)pendingCard.Action != SKDAPI.PendingCardAction.Add)
				{
					InsertPendingCard(cardUID, controllerUID, SKDAPI.PendingCardAction.Delete);
				}
				return true;
			});
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

		public OperationResult<bool> RemoveAccessTemplate(List<Guid> uids)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var cards = Context.Cards.Where(x => uids.Any(y => y == x.UID));
				foreach (var card in cards)
				{
					card.AccessTemplateUID = null;
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<List<GKAPI.GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> deviceDoorUIDs)
		{
			return DbServiceHelper.InTryCatch(() =>
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
							AccessTemplateDoors = x.AccessTemplate.CardDoors,
							GKControllerUIDs = x.GKControllerUIDs
						})
					.OrderBy(x => x.Password)
					.ToList();
				var filteredTableUsers = tableUsers
					.Where(x => x.GKControllerUIDs.Any(gkControllerUID => gkControllerUID.GKControllerUID == deviceUID) || 
						(x.CardDoors.Any(door => deviceDoorUIDs.Any( doorUID => doorUID == door.DoorUID)) ||
						 x.AccessTemplateDoors.Any(door => deviceDoorUIDs.Any( doorUID => doorUID == door.DoorUID))));
				var users = new List<GKAPI.GKUser>();
				foreach (var tableUser in filteredTableUsers)
				{
					var fio = tableUser.LastName != null ? tableUser.LastName : "";
					if (tableUser.FirstName != null && tableUser.FirstName != "")
						fio += " " + tableUser.FirstName;
					if (tableUser.SecondName != null && tableUser.SecondName != "")
						fio += " " + tableUser.SecondName;
					var user = new GKAPI.GKUser
					{
						ExpirationDate = tableUser.ExpirationDate,
						Fio = fio,
						GkLevel = (byte)tableUser.GkLevel,
						GkLevelSchedule = (byte)tableUser.GkLevelSchedule,
						Password = (uint)tableUser.Password,
						UserType = (GKAPI.GKCardType)tableUser.UserType
					};
					var doors = new List<CardDoor>();
					doors.AddRange(tableUser.CardDoors);
					doors.AddRange(tableUser.AccessTemplateDoors.Where(x => !doors.Any(door => door.DoorUID == x.DoorUID)));
					foreach (var cardDoor in doors)
					{
						var door = GKManager.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
						if (door != null)
						{
							if (door.EnterDevice != null)
								user.Descriptors.Add(
									new GKAPI.GKUserDescriptor
									{
										GKDoor = door,
										DescriptorNo = door.EnterDevice.GKDescriptorNo,
										ScheduleNo = cardDoor.EnterScheduleNo
									});
							if (door.DoorType != GKAPI.GKDoorType.OneWay && door.ExitDevice != null)
								user.Descriptors.Add(
									new GKAPI.GKUserDescriptor
									{
										GKDoor = door,
										DescriptorNo = door.ExitDevice.GKDescriptorNo,
										ScheduleNo = cardDoor.ExitScheduleNo
									});
						}
					}
					users.Add(user);
				}
				return users;
			});
		}
	}
}