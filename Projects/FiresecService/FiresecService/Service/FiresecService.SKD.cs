using GKProcessor;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(Guid clientUID, EmployeeFilter filter)
		{
			OperationResult<List<ShortEmployee>> result;
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				result = databaseService.EmployeeTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveEmployee(Guid clientUID, Employee item, bool isNew)
		{
			if (isNew)
			{
				if (item.Type == PersonType.Employee)
					AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_сотрудник);
				else if (item.Type == PersonType.Guest)
					AddJournalMessage(JournalEventNameType.Редактирование_посетителя, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_посетитель);
			}
			else
			{
				if (item.Type == PersonType.Employee)
					AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_сотрудник);
				else if (item.Type == PersonType.Guest)
					AddJournalMessage(JournalEventNameType.Редактирование_посетителя, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_посетитель);
			}
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			if (isEmployee)
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, uid, clientUID, JournalEventDescriptionType.Удаление_сотрудник);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_посетителя, name, uid, clientUID, JournalEventDescriptionType.Удаление_посетитель);
			var errors = new List<string>();
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var getEmployeeOperationResult = databaseService.CardTranslator.GetEmployeeCards(uid);

				if (!getEmployeeOperationResult.HasError)
				{
					foreach (var card in getEmployeeOperationResult.Result)
					{
						var operationResult = DeleteCardFromEmployee(clientUID, card, name, "Сотрудник удален");
						if (operationResult.HasError)
						{
							foreach (var item in operationResult.Errors)
							{
								errors.Add("Ошибка БД: " + item);
							}
						}
					}
				}

				var markdDletedOperationResult = databaseService.EmployeeTranslator.MarkDeleted(uid);
				if (markdDletedOperationResult.HasError)
				{
					errors.Add("Ошибка БД: " + markdDletedOperationResult.Error);
				}
			}

			if (errors.Count > 0)
				return OperationResult<bool>.FromError(errors);
			else
				return new OperationResult<bool>(true);
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracks(filter, startDate, endDate);
			}
		}
		public Stream GetTimeTracksStream(Guid clientUID, EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracksStream(filter, startDate, endDate);
			}
		}
		public OperationResult<bool> SaveEmployeeDepartment(Guid clientUID, Guid uid, Guid? departmentUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, uid, clientUID, JournalEventDescriptionType.Редактирование_сотрудник);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.SaveDepartment(uid, departmentUid);
			}
		}
		public OperationResult<bool> SaveEmployeePosition(Guid clientUID, Guid uid, Guid? PositionUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, uid, clientUID, JournalEventDescriptionType.Редактирование_сотрудник);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.SavePosition(uid, PositionUid);
			}
		}
		public OperationResult<bool> RestoreEmployee(Guid clientUID, Guid uid, string name, bool isEmployee)
		{
			if (isEmployee)
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, uid, clientUID, JournalEventDescriptionType.Восстановление_сотрудник);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_посетителя, name, uid, clientUID, JournalEventDescriptionType.Восстановление_посетитель);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.Restore(uid);
			}
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(Guid clientUID, DepartmentFilter filter)
		{
			OperationResult<List<ShortDepartment>> result;
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				result = databaseService.DepartmentTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<Department> GetDepartmentDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveDepartment(Guid clientUID, Department item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_отдел);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_отдел);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedDepartment(Guid clientUID, ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, department.Name, department.UID, clientUID, JournalEventDescriptionType.Удаление_отдел);
			foreach (var childDepartment in department.ChildDepartments)
			{
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, childDepartment.Name, childDepartment.UID, clientUID, JournalEventDescriptionType.Удаление_отдел);
			}
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(department.UID);
			}
		}
		public OperationResult<bool> SaveDepartmentChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, name, uid, clientUID, JournalEventDescriptionType.Редактирование_отдел);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult<bool> RestoreDepartment(Guid clientUID, ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, department.Name, department.UID, clientUID, JournalEventDescriptionType.Восстановление_отдел);
			foreach (var parent in department.ParentDepartments)
			{
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, parent.Name, parent.UID, clientUID, JournalEventDescriptionType.Восстановление_отдел);
			}
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.Restore(department.UID);
			}
		}

		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetChildEmployeeUIDs(uid);
			}
		}

		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetParentEmployeeUIDs(uid);
			}
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(Guid clientUID, PositionFilter filter)
		{
			OperationResult<List<ShortPosition>> result;
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				result = databaseService.PositionTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<List<Guid>> GetPositionEmployees(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.GetEmployeeUIDs(uid);
			}
		}
		public OperationResult<Position> GetPositionDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SavePosition(Guid clientUID, Position item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_должность);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_должность);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedPosition(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_должности, name, uid, clientUID, JournalEventDescriptionType.Удаление_должность);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestorePosition(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_должности, name, uid, clientUID, JournalEventDescriptionType.Восстановление_должность);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.Restore(uid);
			}
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(Guid clientUID, CardFilter filter)
		{
			var result = new OperationResult<List<SKDCard>>();
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				result = databaseService.CardTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<SKDCard> GetSingleCard(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetSingle(uid);
			}
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid clientUID, Guid employeeUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetEmployeeCards(employeeUID);
			}
		}
		public OperationResult<bool> AddCard(Guid clientUID, SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Добавление_карты, employeeName, card.EmployeeUID, clientUID);

			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error, false);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				var addGKCardResult = AddGKCard(clientUID, card, getAccessTemplateOperationResult.Result, databaseService);
				errors.AddRange(addGKCardResult);

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> AddGKCard(Guid clientUID, SKDCard card, AccessTemplate accessTemplate, RubezhDAL.DataClasses.DbService databaseService)
		{
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.EmployeeUID);
			if (!employeeOperationResult.HasError)
			{
				var accessTemplateCardDoors = accessTemplate != null ? accessTemplate.CardDoors : new List<CardDoor>();
				var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplateCardDoors);
				foreach (var controllerCardSchedule in controllerCardSchedules)
				{
					var addResult = GKSKDHelper.AddOrEditCard(controllerCardSchedule, card, employeeOperationResult.Result.FIO);
					if (addResult.HasError)
					{
						yield return "Не удалось добавить карту в устройство " + controllerCardSchedule.ControllerDevice.PresentationName;
						var pendingResult = databaseService.CardTranslator.AddPending(card.UID, controllerCardSchedule.ControllerDevice.UID);
						if (pendingResult.HasError)
							yield return pendingResult.Error;
					}
				}
			}
		}

		public OperationResult<bool> EditCard(Guid clientUID, SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_карты, employeeName, card.EmployeeUID, clientUID);

			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var oldCardOperationResult = databaseService.CardTranslator.GetSingle(card.UID);

				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error, false);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				if (!oldCardOperationResult.HasError && oldCardOperationResult.Result != null)
				{
					var oldCard = oldCardOperationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					errors.AddRange(EditGKCard(clientUID, oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add("Не найдена предыдущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> EditGKCard(Guid clientUID, SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate, RubezhDAL.DataClasses.DbService databaseService)
		{
			var result = new List<string>();
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(newCard.EmployeeUID);
			if (!employeeOperationResult.HasError)
			{
				var cardDoors = oldAccessTemplate != null ? oldAccessTemplate.CardDoors : new List<CardDoor>();
				var oldControllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(oldCard, cardDoors);
				cardDoors = newAccessTemplate != null ? newAccessTemplate.CardDoors : new List<CardDoor>();
				var newControllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(newCard, cardDoors);

				foreach (var controllerCardSchedule in oldControllerCardSchedules)
				{
					if (!newControllerCardSchedules.Any(x => x.ControllerDevice.UID == controllerCardSchedule.ControllerDevice.UID))
					{
						var removeResult = GKSKDHelper.RemoveCard(controllerCardSchedule.ControllerDevice, newCard);
						if (removeResult.HasError)
						{
							result.Add("Не удалось удалить карту из устройства " + controllerCardSchedule.ControllerDevice.PresentationName);
							var pendingResult = databaseService.CardTranslator.DeletePending(newCard.UID, controllerCardSchedule.ControllerDevice.UID);
							if (pendingResult.HasError)
								result.Add(pendingResult.Error);
						}
					}
				}

				foreach (var controllerCardSchedule in newControllerCardSchedules)
				{
					var addResult = GKSKDHelper.AddOrEditCard(controllerCardSchedule, newCard, employeeOperationResult.Result.FIO);
					if (addResult.HasError)
					{
						result.Add("Не удалось добавить или редактировать карту в устройстве " + controllerCardSchedule.ControllerDevice.PresentationName);
						var pendingResult = databaseService.CardTranslator.AddPending(newCard.UID, controllerCardSchedule.ControllerDevice.UID);
						if (pendingResult.HasError)
							result.Add(pendingResult.Error);
					}
				}
			}
			return result;
		}

		public OperationResult<bool> DeleteCardsFromEmployee(Guid clientUID, List<SKDCard> cards, List<CardAccessTemplateDoors> cardAccessTemplateDoors)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var errors = new List<string>();
				foreach (var card in cards)
				{
					AddJournalMessage(JournalEventNameType.Удаление_карты, card.EmployeeName, card.EmployeeUID, clientUID);
					var cardDoors = new List<CardDoor>();
					var cardAccessTemplateDoor = cardAccessTemplateDoors.FirstOrDefault(x => x.CardUID == card.UID);
					if (cardAccessTemplateDoor != null)
						cardDoors = cardAccessTemplateDoor.CardDoors;
					errors.AddRange(DeleteGKCard(clientUID, card, cardDoors, databaseService));
				}
				return OperationResult<bool>.FromError(errors, true);
			}
		}

		public OperationResult<bool> DeleteCardFromEmployee(Guid clientUID, SKDCard card, string employeeName, string reason = null)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				AddJournalMessage(JournalEventNameType.Удаление_карты, employeeName, card.EmployeeUID, clientUID);

				var saveResult = databaseService.CardTranslator.ToStopList(card, reason);
				if (saveResult.HasError)
				{
					return OperationResult<bool>.FromError(saveResult.Error, false);
				}

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				var operationResult = databaseService.CardTranslator.GetSingle(card.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var cardDoors = getAccessTemplateOperationResult.Result != null ? getAccessTemplateOperationResult.Result.CardDoors : new List<CardDoor>();
					errors.AddRange(DeleteGKCard(clientUID, card, cardDoors, databaseService));
				}
				else
				{
					errors.Add("Не найдена предыдущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> DeleteGKCard(Guid clientUID, SKDCard card, List<CardDoor> accessTemplateDoors, RubezhDAL.DataClasses.DbService databaseService)
		{
			var result = new List<string>();
			var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplateDoors);
			foreach (var controllerCardSchedule in controllerCardSchedules)
			{
				var removeResult = GKSKDHelper.RemoveCard(controllerCardSchedule.ControllerDevice, card);
				if (removeResult.HasError)
				{
					result.Add("Не удалось удалить карту из устройства " + controllerCardSchedule.ControllerDevice.PresentationName);
					var pendingResult = databaseService.CardTranslator.DeletePending(card.UID, controllerCardSchedule.ControllerDevice.UID);
					if (pendingResult.HasError)
						result.Add(pendingResult.Error);
				}
			}
			return result;
		}

		public OperationResult<bool> DeletedCard(Guid clientUID, SKDCard card)
		{
			AddJournalMessage(JournalEventNameType.Удаление_карты, card.EmployeeName, card.EmployeeUID, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				var cardDoors = getAccessTemplateOperationResult.Result != null ? getAccessTemplateOperationResult.Result.CardDoors : new List<CardDoor>();
				errors.AddRange(DeleteGKCard(clientUID, card, cardDoors, databaseService));

				var deleteFromDbResult = databaseService.CardTranslator.Delete(card);
				if (deleteFromDbResult.HasError)
					errors.Add(deleteFromDbResult.Error);

				if (errors.Count > 0)
					return OperationResult<bool>.FromError(errors);
				else
					return new OperationResult<bool>(true);
			}
		}
		public OperationResult<bool> SaveCardTemplate(Guid clientUID, SKDCard card)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.SavePassTemplate(card);
			}
		}

		public OperationResult<List<GKUser>> GetDbDeviceUsers(Guid deviceUID, List<Guid> doorUIDs)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetDbDeviceUsers(deviceUID, doorUIDs);
			}
		}

		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(Guid clientUID, AccessTemplateFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AccessTemplateTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveAccessTemplate(Guid clientUID, AccessTemplate accessTemplate, bool isNew)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, accessTemplate.UID, clientUID, JournalEventDescriptionType.Добавление_шаблона_доступ);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, accessTemplate.UID, clientUID, JournalEventDescriptionType.Редактирование_шаблона_доступ);
				var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(accessTemplate.UID);
				var saveResult = databaseService.AccessTemplateTranslator.Save(accessTemplate);

				var errors = new List<string>();
				if (saveResult.HasError)
					errors.Add(saveResult.Error);

				var operationResult = databaseService.CardTranslator.GetByAccessTemplateUID(accessTemplate.UID);
				if (operationResult.Result != null)
				{
					foreach (var card in operationResult.Result)
					{
						errors.AddRange(EditGKCard(clientUID, card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate, databaseService));
					}
				}

				return OperationResult<bool>.FromError(errors, !saveResult.HasError);
			}
		}
		public OperationResult<List<string>> MarkDeletedAccessTemplate(Guid clientUID, AccessTemplate accessTemplate)
		{
			var operationResult = new OperationResult<List<string>>();
			var warnings = new List<string>();
			AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, accessTemplate.UID, clientUID, JournalEventDescriptionType.Удаление_шаблона_доступ);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var cardsResult = databaseService.CardTranslator.GetByAccessTemplateUID(accessTemplate.UID);
				if (!cardsResult.HasError && cardsResult.Result.IsNotNullOrEmpty())
				{
					var cards = cardsResult.Result;
					var removeFromCardsResult = databaseService.CardTranslator.RemoveAccessTemplate(cards.Select(x => x.UID).ToList());
					if (removeFromCardsResult.HasError)
						return OperationResult<List<string>>.FromError(removeFromCardsResult.Error);
					var cardsToUpdate = cards.Where(x => x.CardDoors.Count > 0).ToList();
					foreach (var card in cardsToUpdate)
					{
						warnings.AddRange(EditGKCard(clientUID, card, accessTemplate, card, null, databaseService));
					}
					var cardsToRemove = cards.Where(x => x.CardDoors.Count == 0).ToList();
					foreach (var card in cardsToRemove)
					{
						warnings.AddRange(DeleteGKCard(clientUID, card, accessTemplate.CardDoors, databaseService));
					}
				}
				var markDeletedResult = databaseService.AccessTemplateTranslator.MarkDeleted(accessTemplate.UID);
				if (markDeletedResult.HasError)
					operationResult = OperationResult<List<string>>.FromError(markDeletedResult.Error);
				operationResult = new OperationResult<List<string>>(warnings);
				return operationResult;
			}
		}

		public OperationResult<bool> RestoreAccessTemplate(Guid clientUID, AccessTemplate accessTemplate)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, accessTemplate.UID, clientUID, JournalEventDescriptionType.Восстановление_шаблона_доступ);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AccessTemplateTranslator.Restore(accessTemplate.UID);
			}
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(Guid clientUID, OrganisationFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveOrganisation(Guid clientUID, OrganisationDetails item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_организация);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedOrganisation(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, uid, clientUID, JournalEventDescriptionType.Удаление_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var errors = new List<string>();
				var cards = databaseService.CardTranslator.Get(
					new CardFilter
					{
						EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { uid } },
						DeactivationType = LogicalDeletationType.Active
					});
				var cardAccessTemplateDoors = databaseService.CardTranslator.GetAccessTemplateDoorsByOrganisation(uid);
				var toStopListResult = databaseService.CardTranslator.ToStopListByOrganisation(uid, "Организация удалена");
				if (toStopListResult.HasError)
					errors.Add(toStopListResult.Error);

				if (!cards.HasError && !cardAccessTemplateDoors.HasError)
				{
					var deleteCardsResult = DeleteCardsFromEmployee(clientUID, cards.Result, cardAccessTemplateDoors.Result);
					if (deleteCardsResult.HasError)
						errors.Add(deleteCardsResult.Error);

					var markDeleledResult = databaseService.OrganisationTranslator.MarkDeleted(uid);
					if (markDeleledResult.HasError)
					{
						errors.Add("Ошибка БД:");
						errors.Add(markDeleledResult.Error);
					}
					if (errors.Count > 0)
						return OperationResult<bool>.FromError(String.Join("\n", errors));
					else
						return new OperationResult<bool>(true);
				}
				else
				{
					errors.Add(cardAccessTemplateDoors.Error);
					errors.Add(cards.Error);
					return OperationResult<bool>.FromError(String.Join("\n", errors));
				}
			}
		}
		public OperationResult<bool> AddOrganisationDoor(Guid clientUID, Organisation item, Guid doorUID)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.AddDoor(item.UID, doorUID);
			}
		}
		public OperationResult<bool> RemoveOrganisationDoor(Guid clientUID, Organisation item, Guid doorUID)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.RemoveDoor(item.UID, doorUID);
			}
		}
		public OperationResult<bool> SaveOrganisationUsers(Guid clientUID, Organisation item)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveUsers(item.UID, item.UserUIDs);
			}
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.GetDetails(uid);
			}
		}
		public OperationResult<bool> SaveOrganisationChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, uid, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveChief(uid, chiefUID);
			}
		}
		public OperationResult<bool> SaveOrganisationHRChief(Guid clientUID, Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, uid, clientUID, JournalEventDescriptionType.Редактирование_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveHRChief(uid, chiefUID);
			}
		}
		public OperationResult<bool> RestoreOrganisation(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, uid, clientUID, JournalEventDescriptionType.Восстановление_организация);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Restore(uid);
			}
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.IsAnyItems(uid);
			}
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(Guid clientUID, AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Get(filter);
			}
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveAdditionalColumnType(Guid clientUID, AdditionalColumnType item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, item.UID, clientUID, JournalEventDescriptionType.Добавление_дополнительная_колонка);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование_дополнительная_колонка);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, name, uid, clientUID, JournalEventDescriptionType.Удаление_дополнительная_колонка);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestoreAdditionalColumnType(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, name, uid, clientUID, JournalEventDescriptionType.Восстановление_дополнительная_колонка);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Restore(uid);
			}
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid clientUID, Guid organisationUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.NightSettingTranslator.GetByOrganisation(organisationUID);
			}
		}

		public OperationResult<bool> SaveNightSettings(Guid clientUID, NightSettings nightSettings)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.NightSettingTranslator.Save(nightSettings);
			}
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(Guid clientUID, PassCardTemplateFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.ShortTranslator.Get(filter);
			}
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.GetPassCardTemplate(uid);
			}
		}
		public OperationResult<bool> SavePassCardTemplate(Guid clientUID, PassCardTemplate item, bool isNew)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, item.Caption, item.UID, clientUID, JournalEventDescriptionType.Добавление_шаблон_пропуска);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, item.Caption, item.UID, clientUID, JournalEventDescriptionType.Редактирование_шаблон_пропуска);
				return databaseService.PassCardTemplateTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedPassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, name, uid, clientUID, JournalEventDescriptionType.Удаление_шаблон_пропуска);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestorePassCardTemplate(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, name, uid, clientUID, JournalEventDescriptionType.Восстановление_шаблон_пропуска);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.Restore(uid);
			}
		}
		#endregion

		#region TestData
		public OperationResult<bool> GenerateEmployeeDays(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TestDataGenerator.GenerateEmployeeDays();
			}
		}

		public OperationResult<bool> GenerateTestData(Guid clientUID, bool isAscending)
		{
			var stoppWatch = new Stopwatch();
			stoppWatch.Start();
			List<Guid> cardUIDs;
			using (var ds = new RubezhDAL.DataClasses.DbService())
			{
				cardUIDs = ds.TestDataGenerator.TestEmployeeCards();
				ds.TestDataGenerator.TestCardDoors(cardUIDs, isAscending);
			}
			stoppWatch.Stop();
			Trace.WriteLine("GenerateTestData " + stoppWatch.Elapsed);

			//bool isBreak = false;
			//int currentPage = 0;
			//int pageSize = 10000;
			//while (!isBreak)
			//{
			//	var cardUIDsportion = cardUIDs.Skip(currentPage * pageSize).Take(pageSize).ToList();
			//	using (var ds = new RubezhDAL.DataClasses.DbService())
			//	{
			//		ds.TestDataGenerator.TestCardDoors(cardUIDsportion, isAscending);
			//	}
			//	isBreak = cardUIDsportion.Count < pageSize;
			//	currentPage++;
			//}

			return new OperationResult<bool>(true);
		}

		public OperationResult<bool> GenerateJournal(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TestDataGenerator.GenerateJournal();
			}
		}
		#endregion

		public OperationResult<bool> SaveJournalVideoUID(Guid clientUID, Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.SaveVideoUID(journalItemUID, videoUID, cameraUID);
			}
		}

		public OperationResult<bool> SaveJournalCameraUID(Guid clientUID, Guid journalItemUID, Guid CameraUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.SaveCameraUID(journalItemUID, CameraUID);
			}
		}

		#region GKSchedule
		/// <summary>
		/// Получение всех графиков ГК
		/// Список графиков един для всей системы и не зависит от конкретного ГК
		/// </summary>
		/// <returns></returns>
		public OperationResult<List<GKSchedule>> GetGKSchedules(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.GKScheduleTranslator.Get();
			}
		}

		/// <summary>
		/// Сохранить график в БД и записать во все ГК
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="isNew"></param>
		/// <returns>Возвращает False, только если произошла ошибка в БД</returns>
		public OperationResult<bool> SaveGKSchedule(Guid clientUID, GKSchedule schedule, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_ГК, schedule.Name, schedule.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_ГК, schedule.Name, schedule.UID, clientUID);

			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var dbResult = databaseService.GKScheduleTranslator.Save(schedule);
				if (dbResult.HasError)
					return OperationResult<bool>.FromError(dbResult.Error, false);
			}
			var result = GKScheduleHelper.SetSchedule(schedule);
			return OperationResult<bool>.FromError(result.Error, true);
		}

		/// <summary>
		/// Удалить график и перезаписать все ГК
		/// </summary>
		/// <param name="schedule"></param>
		/// <returns>Возвращает False, только если произошла ошибка в БД</returns>
		public OperationResult<bool> DeleteGKSchedule(Guid clientUID, GKSchedule schedule)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_ГК, schedule.Name, schedule.UID, clientUID);

			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var dbResult = databaseService.GKScheduleTranslator.Delete(schedule);
				if (dbResult.HasError)
					return OperationResult<bool>.FromError(dbResult.Error, false);
			}
			var result = GKScheduleHelper.RemoveSchedule(schedule.No);
			return OperationResult<bool>.FromError(result.Error, true);
		}
		#endregion

		#region GKDaySchedule

		/// <summary>
		/// Получение всех дневных графиков
		/// </summary>
		/// <returns></returns>
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.GKDayScheduleTranslator.Get();
			}
		}

		/// <summary>
		/// Редактирование дневного графика в БД и запись во все ГК
		/// </summary>
		/// <param name="daySchedule"></param>
		/// <param name="isNew"></param>
		/// <returns>Возвращает False, только если произошла ошибка в БД</returns>
		public OperationResult<bool> SaveGKDaySchedule(Guid clientUID, GKDaySchedule daySchedule, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика_ГК, daySchedule.Name, daySchedule.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика_ГК, daySchedule.Name, daySchedule.UID, clientUID);

			IEnumerable<GKSchedule> changedSchedules;
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var saveResult = databaseService.GKDayScheduleTranslator.Save(daySchedule);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error, false);

				var schedulesResult = databaseService.GKScheduleTranslator.Get();
				if (schedulesResult.HasError)
					return OperationResult<bool>.FromError(schedulesResult.Error, false);
				changedSchedules = schedulesResult.Result.Where(x => x.ScheduleParts.Any(y => y.DayScheduleUID == daySchedule.UID));
			}
			return GKScheduleHelper.SetSchedules(changedSchedules);
		}

		/// <summary>
		/// Редактирование дневного графика из БД и из всех ГК
		/// При изменении дневного графика ГК перезаписываются все графики в ГК, в которых данный дневной график учавствовал
		/// </summary>
		/// <param name="daySchedule"></param>
		/// <returns>Возвращает False, только если произошла ошибка в БД</returns>
		public OperationResult<bool> DeleteGKDaySchedule(Guid clientUID, GKDaySchedule daySchedule)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика_ГК, daySchedule.Name, daySchedule.UID, clientUID);

			IEnumerable<GKSchedule> changedSchedules;
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var schedulesResult = databaseService.GKScheduleTranslator.Get();
				if (schedulesResult.HasError)
					return OperationResult<bool>.FromError(schedulesResult.Error, false);
				changedSchedules = schedulesResult.Result.Where(x => x.ScheduleParts.Any(y => y.DayScheduleUID == daySchedule.UID));

				var deleteResult = databaseService.GKDayScheduleTranslator.Delete(daySchedule);
				if (deleteResult.HasError)
					return OperationResult<bool>.FromError(deleteResult.Error, false);
			}
			return GKScheduleHelper.SetSchedules(changedSchedules);
		}
		#endregion

		#region Export
		public OperationResult<bool> ExportOrganisation(Guid clientUID, ExportFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Export(filter);
			}
		}
		public OperationResult<bool> ImportOrganisation(Guid clientUID, ImportFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Import(filter);
			}
		}

		public OperationResult<bool> ExportOrganisationList(Guid clientUID, ExportFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.ListSynchroniser.Export(filter);
			}
		}
		public OperationResult<bool> ImportOrganisationList(Guid clientUID, ImportFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.ListSynchroniser.Import(filter);
			}
		}

		public OperationResult<bool> ExportJournal(Guid clientUID, JournalExportFilter filter)
		{
			var journalResult = new OperationResult<bool>(true);
			var passJournalResult = new OperationResult<bool>(true);
			if (filter.IsExportJournal)
			{
				using (var databaseService = new RubezhDAL.DataClasses.DbService())
				{
					journalResult = databaseService.JournalTranslator.JournalSynchroniser.Export(filter);
				}
			}
			if (filter.IsExportPassJournal)
			{
				using (var databaseService = new RubezhDAL.DataClasses.DbService())
				{
					passJournalResult = databaseService.PassJournalTranslator.Synchroniser.Export(filter);
				}
			}
			return RubezhDAL.DataClasses.DbServiceHelper.ConcatOperationResults(journalResult, passJournalResult);
		}

		public OperationResult<bool> ExportConfiguration(Guid clientUID, ConfigurationExportFilter filter)
		{
			return ConfigurationSynchroniser.Export(filter);
		}
		#endregion

		#region CurrentConsumption
		public OperationResult<bool> SaveCurrentConsumption(Guid clientUID, CurrentConsumption item)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CurrentConsumptionTranslator.Save(item);
			}
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(Guid clientUID, CurrentConsumptionFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CurrentConsumptionTranslator.Get(filter);
			}
		}
		#endregion
	}
}