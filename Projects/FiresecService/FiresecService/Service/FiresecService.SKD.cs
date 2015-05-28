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
using SKDDriver.Translators;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		#region Employee
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			OperationResult<IEnumerable<ShortEmployee>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.EmployeeTranslator.GetList(filter);
			}
			return result;
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.GetSingle(uid);
			}
		}
		public OperationResult SaveEmployee(Employee item, bool isNew)
		{
			if(isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_сотрудника, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_сотрудника, name, JournalEventDescriptionType.Удаление, uid: uid);
			var errors = new List<string>();
			using (var databaseService = new SKDDatabaseService())
			{
				var getEmployeeOperationResult = databaseService.EmployeeTranslator.GetSingle(uid);

				if (!getEmployeeOperationResult.HasError)
				{
					foreach (var card in getEmployeeOperationResult.Result.Cards)
					{
						var operationResult = DeleteCardFromEmployee(card, "Сотрудник удален");
						if (operationResult.HasError)
						{
							errors.AddRange(operationResult.Errors);
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracks(filter, startDate, endDate);
			}
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracksStream(filter, startDate, endDate);
			}
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SaveDepartment(uid, departmentUid);
			}
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid PositionUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SavePosition(uid, PositionUid);
			}
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_сотрудника, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Restore(uid);
			}
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			OperationResult<IEnumerable<ShortDepartment>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.DepartmentTranslator.GetList(filter);
			}
			return result;
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetSingle(uid);
			}
		}
		public OperationResult SaveDepartment(Department item, bool isNew)
		{
			if(isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_отдела, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(department.UID);
			}
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Restore(department.UID);
			}
		}

		public OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetChildEmployeeUIDs(uid);
			}
		}

		public OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetParentEmployeeUIDs(uid);
			}
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			OperationResult<IEnumerable<ShortPosition>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.PositionTranslator.GetList(filter);
			}
			return result;
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.GetSingle(uid);
			}
		}
		public OperationResult SavePosition(Position item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_должности, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_должности, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_должности, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Restore(uid);
			}
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.Get(filter);
			}
		}
		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.GetEmployeeCards(employeeUID);
			}
		}
		public OperationResult<bool> AddCard(SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Добавление_карты, employeeName, uid: card.EmployeeUID);

			using (var databaseService = new SKDDatabaseService())
			{
				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error, false);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				errors.AddRange(AddStrazhCard(card, getAccessTemplateOperationResult.Result, databaseService));
				errors.AddRange(AddGKCard(card, getAccessTemplateOperationResult.Result, databaseService));

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> AddStrazhCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = ChinaSKDDriver.Processor.AddCard(card, accessTemplate);
			foreach (var error in cardWriter.GetErrors())
				yield return error;

			foreach (var failedControllerUID in cardWriter.GetFailedControllerUIDs())
			{
				var pendingResult = databaseService.CardTranslator.AddPending(card.UID, failedControllerUID);
				if (pendingResult.HasError)
					yield return pendingResult.Error;
			}
		}
		IEnumerable<string> AddGKCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
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

			using (var databaseService = new SKDDatabaseService())
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

					errors.AddRange(EditStrazhCard(oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result, databaseService));
					errors.AddRange(EditGKCard(oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add("Не найдена предыдущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> EditStrazhCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = ChinaSKDDriver.Processor.EditCard(oldCard, oldAccessTemplate, card, accessTemplate);
			foreach (var error in cardWriter.GetErrors())
				yield return error;

			foreach (var failedControllerUID in cardWriter.GetFailedControllerUIDs())
			{
				var pendingResult = databaseService.CardTranslator.EditPending(card.UID, failedControllerUID);
				if (pendingResult.HasError)
					yield return pendingResult.Error;
			}
		}
		IEnumerable<string> EditGKCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate, SKDDatabaseService databaseService)
		{
			var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(newCard.HolderUID);
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
			using (var databaseService = new SKDDatabaseService())
			{
				AddJournalMessage(JournalEventNameType.Удаление_карты, employeeName, uid: card.EmployeeUID);

				var cardToDelete = new SKDCard()
				{
					UID = card.UID,
					Number = card.Number,
					HolderUID = null,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now,
					UserTime = 0,
					DeactivationControllerUID = Guid.Empty,
					CardDoors = new List<CardDoor>(),
					PassCardTemplateUID = null,
					AccessTemplateUID = null,
					CardType = card.CardType,
					GKCardType = card.GKCardType,
					Password = null,
					IsInStopList = true,
					StopReason = reason,
					EmployeeName = null,
					EmployeeUID = Guid.Empty,
					OrganisationUID = Guid.Empty,
					GKLevel = 0,
					GKLevelSchedule = 0
				};

				var saveResult = databaseService.CardTranslator.Save(cardToDelete);
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
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					errors.AddRange(DeleteStrazhCard(card, getAccessTemplateOperationResult.Result, databaseService));
					errors.AddRange(DeleteGKCard(card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add("Не найдена предидущая карта");
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}
		IEnumerable<string> DeleteStrazhCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = ChinaSKDDriver.Processor.DeleteCard(card, accessTemplate);
			foreach (var error in cardWriter.GetErrors())
				yield return error;

			foreach (var failedControllerUID in cardWriter.GetFailedControllerUIDs())
			{
				var pendingResult = databaseService.CardTranslator.DeletePending(card.UID, failedControllerUID);
				if (pendingResult.HasError)
					yield return pendingResult.Error;
			}
		}
		IEnumerable<string> DeleteGKCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.Delete(card.UID);
			}
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.SavePassTemplate(card);
			}
		}

		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate accessTemplate, bool isNew)
		{
			using (var databaseService = new SKDDatabaseService())
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
						errors.AddRange(EditStrazhCard(card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate, databaseService));
						errors.AddRange(EditGKCard(card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate, databaseService));
					}
				}

				return OperationResult<bool>.FromError(errors, !saveResult.HasError);
			}
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.Restore(uid);
			}
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Get(filter);
			}
		}
		public OperationResult SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_организации, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_организации, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();
				var cards = databaseService.CardTranslator.Get(new CardFilter { EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { uid } }, DeactivationType = LogicalDeletationType.Active });
				if (!cards.HasError)
				{
					foreach (var card in cards.Result)
					{
						var cardResult = DeleteCardFromEmployee(card, name);
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.AddDoor(item.UID, doorUID);
			}
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.RemoveDoor(item.UID, doorUID);
			}
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveUsers(item);
			}
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.GetDetails(uid);
			}
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveHRChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_организации, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Restore(uid);
			}
		}

		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.IsAnyItems(uid);
			}
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetList(filter);
			}
		}
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Get(filter);
			}
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
			}
		}
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_дополнительной_колонки, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Restore(uid);
			}
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.NightSettingsTranslator.GetByOrganisation(organisationUID);
			}
		}

		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.NightSettingsTranslator.Save(nightSettings);
			}
		}
		#endregion

		#region Devices
		public OperationResult<SKDStates> SKDGetStates()
		{
			return new OperationResult<SKDStates>(SKDProcessor.SKDGetStates());
		}

		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_контроллера, device);
				return ChinaSKDDriver.Processor.GetDeviceInfo(deviceUID);
			}
			return OperationResult<SKDDeviceInfo>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Синхронизация_времени_контроллера, device);
				return ChinaSKDDriver.Processor.SyncronyseTime(deviceUID);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDResetController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Сброс_Контроллера, device);
				return ChinaSKDDriver.Processor.ResetController(deviceUID);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDRebootController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Перезагрузка_Контроллера, device);
				return ChinaSKDDriver.Processor.RebootController(deviceUID);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_графиков_работы, device);
				return ChinaSKDDriver.Processor.SKDWriteTimeSheduleConfiguration(deviceUID);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			var errors = new List<string>();
			var failedDeviceUIDs = new List<Guid>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					AddSKDJournalMessage(JournalEventNameType.Запись_графиков_работы, device);
					var result = ChinaSKDDriver.Processor.SKDWriteTimeSheduleConfiguration(device.UID);
					if (result.HasError)
					{
						failedDeviceUIDs.Add(device.UID);
						errors.AddRange(result.Errors);
					}
				}
			}
			return OperationResult<List<Guid>>.FromError(errors, failedDeviceUIDs);
		}

		public OperationResult<bool> SKDRewriteAllCards(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				using (var databaseService = new SKDDatabaseService())
				{
					AddSKDJournalMessage(JournalEventNameType.Перезапись_всех_карт, device);
					var cardsResult = databaseService.CardTranslator.Get(new CardFilter());
					var accessTemplatesResult = databaseService.AccessTemplateTranslator.Get(new AccessTemplateFilter());
					if (!cardsResult.HasError && !accessTemplatesResult.HasError)
					{
						return ChinaSKDDriver.Processor.SKDRewriteAllCards(device, cardsResult.Result, accessTemplatesResult.Result);
					}
					else
					{
						return OperationResult<bool>.FromError("Ошибка при получении карт или шаблонов карт");
					}
				}
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Обновление_ПО_Контроллера, device);
				return OperationResult<bool>.FromError("Функция обновления ПО не доступна");
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_двери, device);
				return ChinaSKDDriver.Processor.GetDoorConfiguration(deviceUID);
			}
			return OperationResult<SKDDoorConfiguration>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_конфигурации_двери, device);
				doorConfiguration.OpenAlwaysTimeIndex = 0;
				return ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, doorConfiguration);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_направления_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerDoorType(deviceUID);
			}
			return OperationResult<DoorType>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_направления_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerDoorType(deviceUID, doorType);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_пароля_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerPassword(deviceUID, name, oldPassword, password);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_временных_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerTimeSettings(deviceUID);
			}
			return OperationResult<SKDControllerTimeSettings>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_временных_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerTimeSettings(deviceUID, controllerTimeSettings);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_сетевых_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerNetworkSettings(deviceUID);
			}
			return OperationResult<SKDControllerNetworkSettings>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_сетевых_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerNetworkSettings(deviceUID, controllerNetworkSettings);
			}
			return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_двери, device);
				return ChinaSKDDriver.Processor.OpenDoor(device);
			}
			else
			{
				return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDCloseDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_закрытие_двери, device);
				return ChinaSKDDriver.Processor.CloseDoor(device);
			}
			else
			{
				return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenDeviceForever(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_двери_в_режим_Открыто, device);
				device.SKDDoorConfiguration.OpenAlwaysTimeIndex = 1;
				var result = ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, device.SKDDoorConfiguration);
				if (!result.HasError && device.State != null)
				{
					device.State.OpenAlwaysTimeIndex = 1;
					var skdStates = new SKDStates();
					skdStates.DeviceStates.Add(device.State);
					ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
				}
				return result;
			}
			else
			{
				return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDCloseDeviceForever(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_двери_в_режим_Закрыто, device);
				device.SKDDoorConfiguration.OpenAlwaysTimeIndex = 0;
				var result = ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, device.SKDDoorConfiguration);
				if (!result.HasError && device.State != null)
				{
					device.State.OpenAlwaysTimeIndex = 0;
					var skdStates = new SKDStates();
					skdStates.DeviceStates.Add(device.State);
					ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
				}
				return result;
			}
			else
			{
				return OperationResult<bool>.FromError("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenZone(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_зоны, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						var result = ChinaSKDDriver.Processor.OpenDoor(lockDevice);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
					}
					else
					{
						return OperationResult<bool>.FromError("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}
				return new OperationResult<bool>(true);
			}
			else
			{
				return OperationResult<bool>.FromError("Зона не найдена в конфигурации");
			}
		}
		public OperationResult<bool> SKDCloseZone(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_закрытие_зоны, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						var result = ChinaSKDDriver.Processor.CloseDoor(lockDevice);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
					}
					else
					{
						return OperationResult<bool>.FromError("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}
				return new OperationResult<bool>(true);
			}
			else
			{
				return OperationResult<bool>.FromError("Зона не найдена в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenZoneForever(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_зоны_в_режим_Открыто, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						lockDevice.SKDDoorConfiguration.OpenAlwaysTimeIndex = 1;
						var result = ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
						else if (device.State != null)
						{
							lockDevice.State.OpenAlwaysTimeIndex = 1;
							var skdStates = new SKDStates();
							skdStates.DeviceStates.Add(lockDevice.State);
							ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
						}
					}
					else
					{
						return OperationResult<bool>.FromError("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}
				return new OperationResult<bool>(true);
			}
			else
			{
				return OperationResult<bool>.FromError("Зона не найдена в конфигурации");
			}
		}
		public OperationResult<bool> SKDCloseZoneForever(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_зоны_в_режим_Закрыто, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						lockDevice.SKDDoorConfiguration.OpenAlwaysTimeIndex = 0;
						var result = ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
						else if (device.State != null)
						{
							lockDevice.State.OpenAlwaysTimeIndex = 0;
							var skdStates = new SKDStates();
							skdStates.DeviceStates.Add(lockDevice.State);
							ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
						}
					}
					else
					{
						return OperationResult<bool>.FromError("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}
				return new OperationResult<bool>(true);
			}
			else
			{
				return OperationResult<bool>.FromError("Зона не найдена в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenDoor(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_точки_доступа, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						return ChinaSKDDriver.Processor.OpenDoor(lockDevice);
					}
					else
					{
						return OperationResult<bool>.FromError("Для точки доступа не найден замок");
					}
				}
				else
				{
					return OperationResult<bool>.FromError("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return OperationResult<bool>.FromError("Точка доступа не найдена в конфигурации");
			}
		}
		public OperationResult<bool> SKDCloseDoor(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_точки_доступа, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						return ChinaSKDDriver.Processor.CloseDoor(lockDevice);
					}
					else
					{
						return OperationResult<bool>.FromError("Для точки доступа не найден замок");
					}
				}
				else
				{
					return OperationResult<bool>.FromError("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return OperationResult<bool>.FromError("Точка доступа не найдена в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenDoorForever(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_точки_доступа_в_режим_Открыто, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						lockDevice.SKDDoorConfiguration.OpenAlwaysTimeIndex = 1;
						var result = ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
						if (!result.HasError && lockDevice.State != null)
						{
							lockDevice.State.OpenAlwaysTimeIndex = 1;
							var skdStates = new SKDStates();
							skdStates.DeviceStates.Add(lockDevice.State);
							ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
						}
						return result;
					}
					else
					{
						return OperationResult<bool>.FromError("Для точки доступа не найден замок");
					}
				}
				else
				{
					return OperationResult<bool>.FromError("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return OperationResult<bool>.FromError("Точка доступа не найдена в конфигурации");
			}
		}
		public OperationResult<bool> SKDCloseDoorForever(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_перевод_точки_доступа_в_режим_Закрыто, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						lockDevice.SKDDoorConfiguration.OpenAlwaysTimeIndex = 0;
						var result = ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
						if (!result.HasError && lockDevice.State != null)
						{
							lockDevice.State.OpenAlwaysTimeIndex = 0;
							var skdStates = new SKDStates();
							skdStates.DeviceStates.Add(lockDevice.State);
							ChinaSKDDriver.Processor.OnStatesChanged(skdStates);
						}
						return result;
					}
					else
					{
						return OperationResult<bool>.FromError("Для точки доступа не найден замок");
					}
				}
				else
				{
					return OperationResult<bool>.FromError("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return OperationResult<bool>.FromError("Точка доступа не найдена в конфигурации");
			}
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<IEnumerable<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.GetList(filter);
			}
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.GetSingle(uid);
			}
		}
		public OperationResult SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			using (var databaseService = new SKDDatabaseService())
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_пропуска, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.Restore(uid);
			}
		}
		#endregion

		public OperationResult ResetSKDDatabase()
		{
			return PatchManager.Reset_SKD();
		}

		public OperationResult GenerateEmployeeDays()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.GenerateEmployeeDays();
			}
		}

		public OperationResult GenerateTestData()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.GenerateTestData();
			}
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			using (var journalTranslator = new JounalTranslator())
			{
				return journalTranslator.SaveVideoUID(journalItemUID, videoUID, cameraUID);
			}
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			using (var journalTranslator = new JounalTranslator())
			{
				return journalTranslator.SaveCameraUID(journalItemUID, CameraUID);
			}
		}


		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.GKScheduleTranslator.GetSchedules();
			}
		}
			
		public OperationResult SaveGKSchedule(GKSchedule item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_ГК, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_ГК, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			var result = new OperationResult();
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.GKScheduleTranslator.SaveSchedule(item);
			}
			if (!result.HasError)
				return GKScheduleHelper.AllGKSetSchedule(item);
			else
				return result;
		}

		public OperationResult DeleteGKSchedule(GKSchedule item)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_ГК, item.Name, uid: item.UID);
			var result = new OperationResult();
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.GKScheduleTranslator.DeleteSchedule(item);
			}
			if (!result.HasError)
				return GKScheduleHelper.AllGKRemoveSchedule(item);
			else
				return result;
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.GKScheduleTranslator.GetDaySchedules();
			}
		}

		public OperationResult SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика_ГК, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика_ГК, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			var result = new OperationResult();
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.GKScheduleTranslator.SaveDaySchedule(item);
			}
			if (!result.HasError)
				return GKScheduleHelper.AllGKSetDaySchedule(item);
			else
				return result;
		}

		public OperationResult DeleteGKDaySchedule(GKDaySchedule item)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика_ГК, item.Name, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.GKScheduleTranslator.DeleteDaySchedule(item);
			}
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Export(filter);
			}
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Import(filter);
			}
		}

		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.ListSynchroniser.Export(filter);
			}
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
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
				using (var journalSynchroniser = new JounalSynchroniser())
				{
					journalResult = journalSynchroniser.Export(filter);
				}
			}
			if (filter.IsExportPassJournal)
			{
				using (var databaseService = new SKDDatabaseService())
				{
					passJournalResult = databaseService.PassJournalTranslator.Synchroniser.Export(filter);
				}
			}
			return TranslatiorHelper.ConcatOperationResults(journalResult, passJournalResult);
		}

		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return ConfigurationSynchroniser.Export(filter);
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CurrentConsumptionTranslator.Save(item);
			}
		}
		public OperationResult<IEnumerable<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CurrentConsumptionTranslator.Get(filter);
			}
		}
		#endregion
	}
}