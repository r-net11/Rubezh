using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriver;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver;
using System.Diagnostics;

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
		public OperationResult SaveEmployee(Employee item)
		{
			//foreach (var gkDevice in XManager.DeviceConfiguration.RootDevice.Children)
			//{
			//	GKAddUser(gkDevice.UID);
			//}
			AddSKDJournalMessage(JournalEventNameType.Редактирование_сотрудника);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedEmployee(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_сотрудника);
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
		public OperationResult SaveDepartment(Department item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_отдела);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedDepartment(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_отдела);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(uid);
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
		public OperationResult SavePosition(Position item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_должности);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedPosition(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_должности);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
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
		public OperationResult<bool> AddCard(SKDCard item)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddSKDJournalMessage(JournalEventNameType.Добавление_карты);
				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(item.AccessTemplateUID);

				string cardWriterError = null;
				var cardWriter = ChinaSKDDriver.Processor.AddCard(item, getAccessTemplateOperationResult.Result);
				cardWriterError = cardWriter.GetError();
				var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);
				var pendingResult = databaseService.CardTranslator.AddPendingList(item.UID, failedControllerUIDs);

				var saveResult = databaseService.CardTranslator.Save(item);

				var stringBuilder = new StringBuilder();
				if (!String.IsNullOrEmpty(cardWriterError))
					stringBuilder.AppendLine(cardWriterError);
				if (pendingResult.HasError)
					stringBuilder.AppendLine(pendingResult.Error);
				if (saveResult.HasError)
					stringBuilder.AppendLine(saveResult.Error);
				if (stringBuilder.Length > 0)
					return new OperationResult<bool>(stringBuilder.ToString()) { Result = !saveResult.HasError };
				else
					return new OperationResult<bool>() { Result = !saveResult.HasError };
			}
		}
		public OperationResult<bool> EditCard(SKDCard item)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddSKDJournalMessage(JournalEventNameType.Редактирование_карты);
				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(item.AccessTemplateUID);

				string cardWriterError = null;
				OperationResult pendingResult;

				var operationResult = databaseService.CardTranslator.GetSingle(item.UID);
				if (!operationResult.HasError)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					var cardWriter = ChinaSKDDriver.Processor.EditCard(oldCard, oldGetAccessTemplateOperationResult.Result, item, getAccessTemplateOperationResult.Result);
					cardWriterError = cardWriter.GetError();
					pendingResult = databaseService.CardTranslator.EditPendingList(item.UID, GetFailedControllerUIDs(cardWriter));
				}
				else
				{
					pendingResult = new OperationResult("Не найдена предидущая карта");
				}

				var saveResult = databaseService.CardTranslator.Save(item);

				var stringBuilder = new StringBuilder();
				if (!String.IsNullOrEmpty(cardWriterError))
					stringBuilder.AppendLine(cardWriterError);
				if (pendingResult.HasError)
					stringBuilder.AppendLine(pendingResult.Error);
				if (saveResult.HasError)
					stringBuilder.AppendLine(saveResult.Error);
				if (stringBuilder.Length > 0)
					return new OperationResult<bool>(stringBuilder.ToString()) { Result = !saveResult.HasError };
				else
					return new OperationResult<bool>() { Result = !saveResult.HasError };
			}
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddSKDJournalMessage(JournalEventNameType.Удаление_карты);

				item.AccessTemplateUID = null;
				item.CardDoors = new List<CardDoor>();
				item.PassCardTemplateUID = null;
				item.EmployeeName = null;
				item.HolderUID = null;
				item.IsInStopList = true;
				item.StopReason = reason;
				item.StartDate = DateTime.Now;
				item.EndDate = DateTime.Now;

				string cardWriterError = null;
				OperationResult pendingResult;

				var operationResult = databaseService.CardTranslator.GetSingle(item.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);
					var cardWriter = ChinaSKDDriver.Processor.DeleteCard(oldCard, oldGetAccessTemplateOperationResult.Result);
					cardWriterError = cardWriter.GetError();
					pendingResult = databaseService.CardTranslator.DeletePendingList(oldCard.UID, GetFailedControllerUIDs(cardWriter));
				}
				else
				{
					pendingResult = new OperationResult("Не найдена предидущая карта");
				}

				var saveResult = databaseService.CardTranslator.Save(item);

				var stringBuilder = new StringBuilder();
				if (!String.IsNullOrEmpty(cardWriterError))
					stringBuilder.AppendLine(cardWriterError);
				if (pendingResult.HasError)
					stringBuilder.AppendLine(pendingResult.Error);
				if (saveResult.HasError)
					stringBuilder.AppendLine(saveResult.Error);
				if (stringBuilder.Length > 0)
					return new OperationResult<bool>(stringBuilder.ToString()) { Result = !saveResult.HasError };
				else
					return new OperationResult<bool>() { Result = !saveResult.HasError };
			}
		}

		public OperationResult MarkDeletedCard(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.MarkDeleted(uid);
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
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate accessTemplate)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddSKDJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа);
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
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.MarkDeleted(uid);
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
		public OperationResult SaveOrganisation(OrganisationDetails item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedOrganisation(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveDoors(organisation);
			}
		}
		public OperationResult SaveOrganisationZones(Organisation organisation)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveZones(organisation);
			}
		}
		public OperationResult SaveOrganisationCardTemplates(Organisation organisation)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveCardTemplates(organisation);
			}
		}
		public OperationResult SaveOrganisationGuardZones(Organisation organisation)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveGuardZones(organisation);
			}
		}
		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_организации);
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
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
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
				return ChinaSKDDriver.Processor.GetdeviceInfo(deviceUID);
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
				if (device.State != null)
				{
					device.State.OpenAlwaysTimeIndex = 1;
				}
				return ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, device.SKDDoorConfiguration);
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
				if (device.State != null)
				{
					device.State.OpenAlwaysTimeIndex = 0;
				}
				return ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, device.SKDDoorConfiguration);
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
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == device.IntAddress / 2);
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
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == device.IntAddress / 2);
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
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == device.IntAddress / 2);
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
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == device.IntAddress / 2);
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
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == door.InDevice.IntAddress / 2);
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
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == door.InDevice.IntAddress / 2);
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
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == door.InDevice.IntAddress / 2);
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
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == door.InDevice.IntAddress / 2);
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
	}
}