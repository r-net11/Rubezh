using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriver;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
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
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.GetList(filter);
			}
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
				AddJournalMessage(JournalEventNameType.Добавление_нового_сотрудника, item.Name);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_сотрудника, name, JournalEventDescriptionType.Удаление);
			var stringBuilder = new StringBuilder();
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
							stringBuilder.AppendLine(operationResult.Error);
						}
					}
				}

				var markdDletedOperationResult = databaseService.EmployeeTranslator.MarkDeleted(uid);
				if (markdDletedOperationResult.HasError)
				{
					stringBuilder.AppendLine("Ошибка БД:");
					stringBuilder.AppendLine(markdDletedOperationResult.Error);
				}
			}

			if (stringBuilder.Length > 0)
				return new OperationResult(stringBuilder.ToString());
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
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SaveDepartment(uid, departmentUid);
			}
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid PositionUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SavePosition(uid, PositionUid);
			}
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_сотрудника, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Restore(uid);
			}
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetList(filter);
			}
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
				AddJournalMessage(JournalEventNameType.Добавление_нового_отдела, item.Name);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedDepartment(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_отдела, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreDepartment(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_отдела, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Restore(uid);
			}
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.GetList(filter);
			}
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
				AddJournalMessage(JournalEventNameType.Добавление_новой_должности, item.Name);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_должности, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_должности, name);
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
		public OperationResult<bool> AddCard(SKDCard card)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();

				AddJournalMessage(JournalEventNameType.Добавление_карты, card.Number.ToString());
				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.Add(getAccessTemplateOperationResult.Error);

				var cardWriter = ChinaSKDDriver.Processor.AddCard(card, getAccessTemplateOperationResult.Result);
				var cardWriterError = cardWriter.GetError();
				if(!String.IsNullOrEmpty(cardWriterError))
					errors.Add(cardWriterError);

				var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);

				var pendingResult = databaseService.CardTranslator.AddPendingList(card.UID, failedControllerUIDs);
				if (pendingResult.HasError)
					errors.Add(pendingResult.Error);

				var saveResult = databaseService.CardTranslator.Save(card);
				if(saveResult.HasError)
					errors.Add(saveResult.Error);

				var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
				if (!employeeOperationResult.HasError)
				{
					var gkSKDHelper = new GKSKDHelper();
					foreach (var gkControllerDevice in GKManager.DeviceConfiguration.RootDevice.Children)
					{
						var addGKResult = gkSKDHelper.AddOneCard(gkControllerDevice, card, getAccessTemplateOperationResult.Result, employeeOperationResult.Result.FIO);
						if (addGKResult.HasError)
						{
							errors.Add("Не удалось добавить карту в устройство " + gkControllerDevice.PresentationName);
							var pendingGkResult = databaseService.CardTranslator.AddPendingList(card.UID, new List<Guid>() { gkControllerDevice.UID });
							if (pendingGkResult.HasError)
								errors.Add(pendingGkResult.Error);
						}
					}
				}

				if (errors.Count > 0)
					return new OperationResult<bool>(String.Join("\n", errors));
				else
					return new OperationResult<bool>() { Result = true };
			}
		}
		public OperationResult<bool> EditCard(SKDCard card)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();

				AddJournalMessage(JournalEventNameType.Редактирование_карты, card.Number.ToString());
				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.Add(getAccessTemplateOperationResult.Error);

				var operationResult = databaseService.CardTranslator.GetSingle(card.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					var cardWriter = ChinaSKDDriver.Processor.EditCard(oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result);
					var cardWriterError = cardWriter.GetError();
					if (!String.IsNullOrEmpty(cardWriterError))
						errors.Add(cardWriterError);

					var pendingResult = databaseService.CardTranslator.EditPendingList(card.UID, GetFailedControllerUIDs(cardWriter));
					if (pendingResult.HasError)
						errors.Add(pendingResult.Error);
				}
				else
				{
					errors.Add("Не найдена предыдущая карта");
				}

				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					errors.Add(saveResult.Error);

				var employeeOperationResult = databaseService.EmployeeTranslator.GetSingle(card.HolderUID);
				if (!employeeOperationResult.HasError)
				{
					var gkSKDHelper = new GKSKDHelper();
					foreach (var gkControllerDevice in GKManager.DeviceConfiguration.RootDevice.Children)
					{
						var addGKResult = gkSKDHelper.AddOneCard(gkControllerDevice, card, getAccessTemplateOperationResult.Result, employeeOperationResult.Result.FIO);
						if (addGKResult.HasError)
						{
							errors.Add("Не удалось изменить карту в устройстве " + gkControllerDevice.PresentationName);
							var pendingGkResult = databaseService.CardTranslator.EditPendingList(card.UID, new List<Guid>() { gkControllerDevice.UID });
							if (pendingGkResult.HasError)
								errors.Add(pendingGkResult.Error);
						}
					}
				}

				if (errors.Count > 0)
					return new OperationResult<bool>(String.Join("\n", errors));
				else
					return new OperationResult<bool>() { Result = true };
			}
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard card, string reason = null)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();

				AddJournalMessage(JournalEventNameType.Удаление_карты, card.Number.ToString());

				card.AccessTemplateUID = null;
				card.CardDoors = new List<CardDoor>();
				card.PassCardTemplateUID = null;
				card.EmployeeName = null;
				card.HolderUID = null;
				card.IsInStopList = true;
				card.StopReason = reason;
				card.StartDate = DateTime.Now;
				card.EndDate = DateTime.Now;

				var operationResult = databaseService.CardTranslator.GetSingle(card.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					var cardWriter = ChinaSKDDriver.Processor.DeleteCard(oldCard, oldGetAccessTemplateOperationResult.Result);
					var cardWriterError = cardWriter.GetError();
					if (!String.IsNullOrEmpty(cardWriterError))
						errors.Add(cardWriterError);

					var pendingResult = databaseService.CardTranslator.DeletePendingList(oldCard.UID, GetFailedControllerUIDs(cardWriter));
					if (pendingResult.HasError)
						errors.Add(pendingResult.Error);
				}
				else
				{
					errors.Add("Не найдена предидущая карта");
				}

				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
				{
					errors.Add("Ошибка БД:");
					errors.Add(saveResult.Error);
				}

				var gkSKDHelper = new GKSKDHelper();
				foreach (var gkControllerDevice in GKManager.DeviceConfiguration.RootDevice.Children)
				{
					var removeGKCardResult = gkSKDHelper.RemoveOneCard(gkControllerDevice, card);
					if (removeGKCardResult.HasError)
					{
						errors.Add("Не удалось удалить карту из устройства " + gkControllerDevice.PresentationName);
						var pendingGkResult = databaseService.CardTranslator.DeletePendingList(card.UID, new List<Guid>() { gkControllerDevice.UID });
						if (pendingGkResult.HasError)
							errors.Add(pendingGkResult.Error);
					}
				}

				if (errors.Count > 0)
					return new OperationResult<bool>(String.Join("\n", errors));
				else
					return new OperationResult<bool>() { Result = true };
			}
		}

		public OperationResult DeletedCard(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.Delete(uid);
			}
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.SavePassTemplate(card);
			}
		}

		IEnumerable<Guid> GetFailedControllerUIDs(CardWriter cardWriter)
		{
			return cardWriter.ControllerCardItems.Where(x => x.HasError).Select(x => x.ControllerDevice.UID);
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
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_доступа, accessTemplate.Name);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, accessTemplate.Name, JournalEventDescriptionType.Редактирование);
				var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(accessTemplate.UID);
				var saveResult = databaseService.AccessTemplateTranslator.Save(accessTemplate);

				var stringBuilder = new StringBuilder();
				if (saveResult.HasError)
					stringBuilder.AppendLine(saveResult.Error);

				var operationResult = databaseService.CardTranslator.GetByAccessTemplateUID(accessTemplate.UID);
				if (operationResult.Result != null)
				{
					foreach (var card in operationResult.Result)
					{
						var cardWriter = ChinaSKDDriver.Processor.EditCard(card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate);
						var cardWriterError = cardWriter.GetError();
						var pendingResult = databaseService.CardTranslator.EditPendingList(accessTemplate.UID, GetFailedControllerUIDs(cardWriter));

						if (!String.IsNullOrEmpty(cardWriterError))
							stringBuilder.AppendLine(cardWriterError);
						if (pendingResult.HasError)
							stringBuilder.AppendLine(pendingResult.Error);
					}
				}

				if (stringBuilder.Length > 0)
					return new OperationResult<bool>(stringBuilder.ToString()) { Result = !saveResult.HasError };
				else
					return new OperationResult<bool>() { Result = !saveResult.HasError };
			}
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_доступа, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_доступа, name);
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
				AddJournalMessage(JournalEventNameType.Добавление_новой_организации, item.Name);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_организации, name);
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();
				var cards = databaseService.CardTranslator.Get(new CardFilter { EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { uid } } });
				if (!cards.HasError)
				{
					foreach (var card in cards.Result)
					{
						var cardResult = DeleteCardFromEmployee(card);
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
		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, organisation.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveDoors(organisation);
			}

		}
		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, organisation.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveUsers(organisation);
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
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveHRChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_организации, name);
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
				AddJournalMessage(JournalEventNameType.Добавление_новой_дополнительной_колонки, item.Name);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дополнительной_колонки, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дополнительной_колонки, name);
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
			return new OperationResult<SKDStates>() { Result = SKDProcessor.SKDGetStates() };
		}

		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_контроллера, device);
				return ChinaSKDDriver.Processor.GetDeviceInfo(deviceUID);
			}
			return new OperationResult<SKDDeviceInfo>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Синхронизация_времени_контроллера, device);
				return ChinaSKDDriver.Processor.SyncronyseTime(deviceUID);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDResetController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Сброс_Контроллера, device);
				return ChinaSKDDriver.Processor.ResetController(deviceUID);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDRebootController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Перезагрузка_Контроллера, device);
				return ChinaSKDDriver.Processor.RebootController(deviceUID);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_графиков_работы, device);
				return ChinaSKDDriver.Processor.SKDWriteTimeSheduleConfiguration(deviceUID);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			var errors = "";
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
						errors += result.Error + " (" + device.Name + ")\n";
					}
				}
			}
			if (string.IsNullOrEmpty(errors))
				return new OperationResult<List<Guid>>() { Result = new List<Guid>() };
			else return new OperationResult<List<Guid>>(errors) { Result = failedDeviceUIDs };
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
						return new OperationResult<bool>("Ошибка при получении карт или шаблонов карт");
					}
				}
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Обновление_ПО_Контроллера, device);
				return new OperationResult<bool>("Функция обновления ПО не доступна");
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_двери, device);
				return ChinaSKDDriver.Processor.GetDoorConfiguration(deviceUID);
			}
			return new OperationResult<SKDDoorConfiguration>("Устройство не найдено в конфигурации");
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
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_направления_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerDoorType(deviceUID);
			}
			return new OperationResult<DoorType>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_направления_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerDoorType(deviceUID, doorType);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_пароля_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerPassword(deviceUID, name, oldPassword, password);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_временных_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerTimeSettings(deviceUID);
			}
			return new OperationResult<SKDControllerTimeSettings>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_временных_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerTimeSettings(deviceUID, controllerTimeSettings);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_сетевых_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.GetControllerNetworkSettings(deviceUID);
			}
			return new OperationResult<SKDControllerNetworkSettings>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_сетевых_настроек_контроллера, device);
				return ChinaSKDDriver.Processor.SetControllerNetworkSettings(deviceUID, controllerNetworkSettings);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
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
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
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
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
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
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
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
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
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
							errors.Add(result.Error);
						}
					}
					else
					{
						return new OperationResult<bool>("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return new OperationResult<bool>(String.Join("\n", errors));
				}
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Зона не найдена в конфигурации");
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
							errors.Add(result.Error);
						}
					}
					else
					{
						return new OperationResult<bool>("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return new OperationResult<bool>(String.Join("\n", errors));
				}
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Зона не найдена в конфигурации");
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
							errors.Add(result.Error);
						}
					}
					else
					{
						return new OperationResult<bool>("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return new OperationResult<bool>(String.Join("\n", errors));
				}
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Зона не найдена в конфигурации");
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
							errors.Add(result.Error);
						}
					}
					else
					{
						return new OperationResult<bool>("Для зоны не найден замок");
					}
				}
				if (errors.Count > 0)
				{
					return new OperationResult<bool>(String.Join("\n", errors));
				}
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Зона не найдена в конфигурации");
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
						return new OperationResult<bool>("Для точки доступа не найден замок");
					}
				}
				else
				{
					return new OperationResult<bool>("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return new OperationResult<bool>("Точка доступа не найдена в конфигурации");
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
						return new OperationResult<bool>("Для точки доступа не найден замок");
					}
				}
				else
				{
					return new OperationResult<bool>("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return new OperationResult<bool>("Точка доступа не найдена в конфигурации");
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
						return ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
					}
					else
					{
						return new OperationResult<bool>("Для точки доступа не найден замок");
					}
				}
				else
				{
					return new OperationResult<bool>("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return new OperationResult<bool>("Точка доступа не найдена в конфигурации");
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
						return ChinaSKDDriver.Processor.SetDoorConfiguration(lockDevice.UID, lockDevice.SKDDoorConfiguration);
					}
					else
					{
						return new OperationResult<bool>("Для точки доступа не найден замок");
					}
				}
				else
				{
					return new OperationResult<bool>("У точки доступа не указано устройство входа");
				}
			}
			else
			{
				return new OperationResult<bool>("Точка доступа не найдена в конфигурации");
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
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_пропуска, item.Caption);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, item.Caption, JournalEventDescriptionType.Редактирование);
				return databaseService.PassCardTemplateTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_пропуска, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_пропуска, name);
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
	}
}