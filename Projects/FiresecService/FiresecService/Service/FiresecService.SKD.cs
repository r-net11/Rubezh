using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using GKProcessor;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			OperationResult<List<ShortEmployee>> result;
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				result = databaseService.EmployeeTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			if(isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_сотрудника, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_сотрудника, name, JournalEventDescriptionType.Удаление, uid: uid);
			var errors = new List<string>();
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				var getEmployeeOperationResult = databaseService.CardTranslator.GetEmployeeCards(uid);

				if (!getEmployeeOperationResult.HasError)
				{
					foreach (var card in getEmployeeOperationResult.Result)
					{
						var operationResult = DeleteCardFromEmployee(card, name, "Сотрудник удален");
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
				return new OperationResult(errors);
			else
				return new OperationResult();
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracks(filter, startDate, endDate);
			}
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracksStream(filter, startDate, endDate);
			}
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.SaveDepartment(uid, departmentUid);
			}
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? PositionUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.SavePosition(uid, PositionUid);
			}
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_сотрудника, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.EmployeeTranslator.Restore(uid);
			}
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			OperationResult<List<ShortDepartment>> result;
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				result = databaseService.DepartmentTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			if(isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_отдела, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Удаление_отдела, department.Name, uid: department.UID);	
			foreach (var childDepartment in department.ChildDepartments)
			{
				AddJournalMessage(JournalEventNameType.Удаление_отдела, childDepartment.Value, uid: childDepartment.Key);	
			}
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(department.UID);
			}
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreDepartment(ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_отдела, department.Name, uid: department.UID);
			foreach (var parent in department.ParentDepartments)
			{
				AddJournalMessage(JournalEventNameType.Восстановление_отдела, parent.Value, uid: parent.Key);
			}
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.Restore(department.UID);
			}
		}

		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetChildEmployeeUIDs(uid);
			}
		}

		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.DepartmentTranslator.GetParentEmployeeUIDs(uid);
			}
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			OperationResult<List<ShortPosition>> result;
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				result = databaseService.PositionTranslator.ShortTranslator.Get(filter);
			}
			return result;
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_должности, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_должности, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_должности, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PositionTranslator.Restore(uid);
			}
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CardTranslator.Get(filter);
			}
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetSingle(uid);
			}
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetEmployeeCards(employeeUID);
			}
		}
		public OperationResult<bool> AddCard(SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Добавление_карты, employeeName, uid: card.EmployeeUID);

			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error, false);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				errors.AddRange(AddGKCard(card, getAccessTemplateOperationResult.Result, databaseService));

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> AddGKCard(SKDCard card, AccessTemplate accessTemplate, SKDDriver.DataClasses.DbService databaseService)
		{
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.EmployeeUID);
			if (!employeeOperationResult.HasError)
			{
				var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplate);
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

		public OperationResult<bool> EditCard(SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_карты, employeeName, uid: card.EmployeeUID);

			using (var databaseService = new SKDDriver.DataClasses.DbService())
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

					errors.AddRange(EditGKCard(oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add("Не найдена предыдущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> EditGKCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate, SKDDriver.DataClasses.DbService databaseService)
		{
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(newCard.EmployeeUID);
			if (!employeeOperationResult.HasError)
			{
				var oldControllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(oldCard, oldAccessTemplate);
				var newControllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(newCard, newAccessTemplate);

				foreach (var controllerCardSchedule in oldControllerCardSchedules)
				{
					if (!newControllerCardSchedules.Any(x => x.ControllerDevice.UID == controllerCardSchedule.ControllerDevice.UID))
					{
						var removeResult = GKSKDHelper.RemoveCard(controllerCardSchedule.ControllerDevice, newCard);
						if (removeResult.HasError)
						{
							yield return "Не удалось удалить карту из устройства " + controllerCardSchedule.ControllerDevice.PresentationName;
							var pendingResult = databaseService.CardTranslator.DeletePending(newCard.UID, controllerCardSchedule.ControllerDevice.UID);
							if (pendingResult.HasError)
								yield return pendingResult.Error;
						}
					}
				}

				foreach (var controllerCardSchedule in newControllerCardSchedules)
				{
					var addResult = GKSKDHelper.AddOrEditCard(controllerCardSchedule, newCard, employeeOperationResult.Result.FIO);
					if (addResult.HasError)
					{
						yield return "Не удалось добавить или редактировать карту в устройстве " + controllerCardSchedule.ControllerDevice.PresentationName;
						var pendingResult = databaseService.CardTranslator.AddPending(newCard.UID, controllerCardSchedule.ControllerDevice.UID);
						if (pendingResult.HasError)
							yield return pendingResult.Error;
					}
				}
			}
		}

		public OperationResult<bool> DeleteCardFromEmployee(SKDCard card, string employeeName, string reason = null)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				AddJournalMessage(JournalEventNameType.Удаление_карты, employeeName, uid: card.EmployeeUID);

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
					errors.AddRange(DeleteGKCard(card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add("Не найдена предидущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> DeleteGKCard(SKDCard card, AccessTemplate accessTemplate, SKDDriver.DataClasses.DbService databaseService)
		{
			var controllerCardSchedules = GKSKDHelper.GetGKControllerCardSchedules(card, accessTemplate);
			foreach (var controllerCardSchedule in controllerCardSchedules)
			{
				var removeResult = GKSKDHelper.RemoveCard(controllerCardSchedule.ControllerDevice, card);
				if (removeResult.HasError)
				{
					yield return "Не удалось удалить карту из устройства " + controllerCardSchedule.ControllerDevice.PresentationName;
					var pendingResult = databaseService.CardTranslator.DeletePending(card.UID, controllerCardSchedule.ControllerDevice.UID);
					if (pendingResult.HasError)
						yield return pendingResult.Error;
				}
			}
		}

		public OperationResult DeletedCard(SKDCard card)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_карты, card.Number.ToString(), uid: card.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CardTranslator.Delete(card);
			}
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CardTranslator.SavePassTemplate(card);
			}
		}

		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AccessTemplateTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate accessTemplate, bool isNew)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_доступа, accessTemplate.Name, uid: accessTemplate.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, JournalEventDescriptionType.Редактирование, uid: accessTemplate.UID);
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
						errors.AddRange(EditGKCard(card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate, databaseService));
					}
				}

				return OperationResult<bool>.FromError(errors, !saveResult.HasError);
			}
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AccessTemplateTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AccessTemplateTranslator.Restore(uid);
			}
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_организации, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_организации, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				var errors = new List<string>();
				var cards = databaseService.CardTranslator.Get(new CardFilter { EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { uid } }, DeactivationType = LogicalDeletationType.Active });
				if (!cards.HasError)
				{
					foreach (var card in cards.Result)
					{
						var cardResult = DeleteCardFromEmployee(card, name, "Огранизация удалена");
						if (cardResult.HasError)
							errors.Add(cardResult.Error);
					}
					var markDeleledResult = databaseService.OrganisationTranslator.MarkDeleted(uid);
					if (markDeleledResult.HasError)
					{
						errors.Add("Ошибка БД:");
						errors.Add(markDeleledResult.Error);
					}
					if (errors.Count > 0)
						return new OperationResult(String.Join("\n", errors));
					else
						return new OperationResult();
				}
				else
				{
					return new OperationResult(cards.Error);
				}
			}
		}
		public OperationResult AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.AddDoor(item.UID, doorUID);
			}
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.RemoveDoor(item.UID, doorUID);
			}
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveUsers(item.UID, item.UserUIDs);
			}
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.GetDetails(uid);
			}
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.SaveHRChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_организации, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Restore(uid);
			}
		}

		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.IsAnyItems(uid);
			}
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Get(filter);
			}
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_дополнительной_колонки, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Restore(uid);
			}
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.NightSettingTranslator.GetByOrganisation(organisationUID);
			}
		}

		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.NightSettingTranslator.Save(nightSettings);
			}
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.ShortTranslator.Get(filter);
			}
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.GetSingle(uid);
			}
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_пропуска, item.Caption, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, item.Caption, JournalEventDescriptionType.Редактирование, uid: item.UID);
				return databaseService.PassCardTemplateTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_пропуска, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_пропуска, name, uid: uid);
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.PassCardTemplateTranslator.Restore(uid);
			}
		}
		#endregion

		#region TestData
		public OperationResult GenerateEmployeeDays()
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.TestDataGenerator.GenerateEmployeeDays();
			}
		}

		public OperationResult GenerateTestData(bool isAscending)
		{
			List<Guid> cardUIDs;
			using (var ds = new SKDDriver.DataClasses.DbService())
			{
				cardUIDs = ds.TestDataGenerator.TestEmployeeCards();
			}
			bool isBreak = false;
			int currentPage = 0;
			int pageSize = 10000;
			while (!isBreak)
			{
				var cardUIDsportion = cardUIDs.Skip(currentPage * pageSize).Take(pageSize).ToList();
				using (var ds = new SKDDriver.DataClasses.DbService())
				{
					ds.TestDataGenerator.TestCardDoors(cardUIDsportion, isAscending);
				}
				isBreak = cardUIDsportion.Count < pageSize;
				currentPage++;
			}

			return new OperationResult();
		}

		public OperationResult GenerateJournal()
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.TestDataGenerator.GenerateJournal();
			}
		}
		#endregion
		

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.SaveVideoUID(journalItemUID, videoUID, cameraUID);
			}
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<bool> SaveGKSchedule(GKSchedule schedule, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_ГК, schedule.Name, uid: schedule.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_ГК, schedule.Name, JournalEventDescriptionType.Редактирование, uid: schedule.UID);

			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<bool> DeleteGKSchedule(GKSchedule schedule)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_ГК, schedule.Name, uid: schedule.UID);

			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule daySchedule, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика_ГК, daySchedule.Name, uid: daySchedule.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика_ГК, daySchedule.Name, JournalEventDescriptionType.Редактирование, uid: daySchedule.UID);

			IEnumerable<GKSchedule> changedSchedules;
			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule daySchedule)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика_ГК, daySchedule.Name, uid: daySchedule.UID);

			IEnumerable<GKSchedule> changedSchedules;
			using (var databaseService = new SKDDriver.DataClasses.DbService())
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
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Export(filter);
			}
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Import(filter);
			}
		}

		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.ListSynchroniser.Export(filter);
			}
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.ListSynchroniser.Import(filter);
			}
		}

		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			var journalResult = new OperationResult();
			var passJournalResult = new OperationResult();
			if (filter.IsExportJournal)
			{
				using (var databaseService = new  SKDDriver.DataClasses.DbService())
				{
					journalResult = databaseService.JournalTranslator.JournalSynchroniser.Export(filter);
				}
			}
			if (filter.IsExportPassJournal)
			{
				using (var databaseService = new SKDDriver.DataClasses.DbService())
				{
					passJournalResult = databaseService.PassJournalTranslator.Synchroniser.Export(filter);
				}
			}
			return SKDDriver.DataClasses.DbServiceHelper.ConcatOperationResults(journalResult, passJournalResult);
		}

		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return ConfigurationSynchroniser.Export(filter);
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CurrentConsumptionTranslator.Save(item);
			}
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter filter)
		{
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				return databaseService.CurrentConsumptionTranslator.Get(filter);
			}
		}
		#endregion
	}
}