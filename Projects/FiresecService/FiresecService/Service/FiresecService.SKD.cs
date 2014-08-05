using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriver;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver;
using FiresecClient;
using System.Text;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		#region Employee
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SKDDatabaseService.EmployeeTranslator.GetList(filter);
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SKDDatabaseService.EmployeeTranslator.GetSingle(uid);
		}
		public OperationResult SaveEmployee(Employee item)
		{
			//foreach (var gkDevice in XManager.DeviceConfiguration.RootDevice.Children)
			//{
			//    GKAddUser(gkDevice.UID);
			//}
			return SKDDatabaseService.EmployeeTranslator.Save(item);
		}
		public OperationResult MarkDeletedEmployee(Guid uid)
		{
			var stringBuilder = new StringBuilder();
			var getEmployeeOperationResult = SKDDatabaseService.EmployeeTranslator.GetSingle(uid);
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
			var markdDletedOperationResult = SKDDatabaseService.EmployeeTranslator.MarkDeleted(uid);

			if (stringBuilder.Length > 0)
				return new OperationResult(stringBuilder.ToString());
			else
				return new OperationResult();
		}
		public OperationResult<List<DayTimeTrack>> GetEmployeeTimeTracks(Guid employeeUID, DateTime startDate, DateTime endDate)
		{
			return SKDDatabaseService.EmployeeTranslator.GetTimeTracks(employeeUID, startDate, endDate);
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SKDDatabaseService.DepartmentTranslator.GetList(filter);
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SKDDatabaseService.DepartmentTranslator.GetSingle(uid);
		}
		public OperationResult SaveDepartment(Department item)
		{
			return SKDDatabaseService.DepartmentTranslator.Save(item);
		}
		public OperationResult MarkDeletedDepartment(Guid uid)
		{
			return SKDDatabaseService.DepartmentTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SKDDatabaseService.PositionTranslator.GetList(filter);
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SKDDatabaseService.PositionTranslator.GetSingle(uid);
		}
		public OperationResult SavePosition(Position item)
		{
			return SKDDatabaseService.PositionTranslator.Save(item);
		}
		public OperationResult MarkDeletedPosition(Guid uid)
		{
			return SKDDatabaseService.PositionTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SKDDatabaseService.CardTranslator.Get(filter);
		}
		public OperationResult<bool> AddCard(SKDCard item)
		{
			AddSKDJournalMessage(JournalEventNameType.Добавление_карты);
			var getAccessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(item.AccessTemplateUID);

			string cardWriterError = null;
			var cardWriter = ChinaSKDDriver.Processor.AddCard(item, getAccessTemplateOperationResult.Result);
			cardWriterError = cardWriter.GetError();
			var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);
			var pendingResult = SKDDatabaseService.CardTranslator.AddPendingList(item.UID, failedControllerUIDs);

			var saveResult = SKDDatabaseService.CardTranslator.Save(item);

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
		public OperationResult<bool> EditCard(SKDCard item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_карты);
			var getAccessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(item.AccessTemplateUID);

			string cardWriterError = null;
			OperationResult pendingResult;

			var operationResult = SKDDatabaseService.CardTranslator.GetSingle(item.UID);
			if (!operationResult.HasError)
			{
				var oldCard = operationResult.Result;
				var oldGetAccessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

				var cardWriter = ChinaSKDDriver.Processor.EditCard(oldCard, oldGetAccessTemplateOperationResult.Result, item, getAccessTemplateOperationResult.Result);
				cardWriterError = cardWriter.GetError();
				pendingResult = SKDDatabaseService.CardTranslator.EditPendingList(item.UID, GetFailedControllerUIDs(cardWriter));
			}
			else
			{
				pendingResult = new OperationResult("Не найдена предидущая карта");
			}

			var saveResult = SKDDatabaseService.CardTranslator.Save(item);

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
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			item.AccessTemplateUID = null;
			item.CardDoors = new List<CardDoor>();
			item.PassCardTemplateUID = null;
			item.EmployeeName = null;
			item.HolderUID = null;
			item.IsInStopList = true;
			item.StopReason = reason;
			item.StartDate = DateTime.Now;
			item.EndDate = DateTime.Now;
			AddSKDJournalMessage(JournalEventNameType.Удаление_карты);

			string cardWriterError = null;
			OperationResult pendingResult;

			var operationResult = SKDDatabaseService.CardTranslator.GetSingle(item.UID);
			if (!operationResult.HasError && operationResult.Result != null)
			{
				var oldCard = operationResult.Result;
				var oldGetAccessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);
				var cardWriter = ChinaSKDDriver.Processor.DeleteCard(oldCard, oldGetAccessTemplateOperationResult.Result);
				cardWriterError = cardWriter.GetError();
				pendingResult = SKDDatabaseService.CardTranslator.DeletePendingList(oldCard.UID, GetFailedControllerUIDs(cardWriter));
			}
			else
			{
				pendingResult = new OperationResult("Не найдена предидущая карта");
			}

			var saveResult = SKDDatabaseService.CardTranslator.Save(item);

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

		public OperationResult MarkDeletedCard(Guid uid)
		{
			return SKDDatabaseService.CardTranslator.MarkDeleted(uid);
		}
		public OperationResult DeletedCard(Guid uid)
		{
			return SKDDatabaseService.CardTranslator.Delete(uid);
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			return SKDDatabaseService.CardTranslator.SavePassTemplate(card);
		}

		IEnumerable<Guid> GetFailedControllerUIDs(CardWriter cardWriter)
		{
			return cardWriter.ControllerCardItems.Where(x => x.HasError).Select(x => x.ControllerDevice.UID);
		}
		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Get(filter);
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate accessTemplate)
		{
			var oldGetAccessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(accessTemplate.UID);
			var saveResult = SKDDatabaseService.AccessTemplateTranslator.Save(accessTemplate);

			var stringBuilder = new StringBuilder();
			if (saveResult.HasError)
				stringBuilder.AppendLine(saveResult.Error);

			var operationResult = SKDDatabaseService.CardTranslator.GetByAccessTemplateUID(accessTemplate.UID);
			if (operationResult.Result != null)
			{
				foreach (var card in operationResult.Result)
				{
					var cardWriter = ChinaSKDDriver.Processor.EditCard(card, oldGetAccessTemplateOperationResult.Result, card, accessTemplate);
					var cardWriterError = cardWriter.GetError();
					var pendingResult = SKDDatabaseService.CardTranslator.EditPendingList(accessTemplate.UID, GetFailedControllerUIDs(cardWriter));

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
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			var result = SKDDatabaseService.AccessTemplateTranslator.MarkDeleted(uid);
			return result;
		}

		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SKDDatabaseService.OrganisationTranslator.Get(filter);
		}
		public OperationResult SaveOrganisation(OrganisationDetails item)
		{
			return SKDDatabaseService.OrganisationTranslator.Save(item);
		}
		public OperationResult MarkDeletedOrganisation(Guid uid)
		{
			return SKDDatabaseService.OrganisationTranslator.MarkDeleted(uid);
		}
		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveDoors(organisation);
		}
		public OperationResult SaveOrganisationZones(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveZones(organisation);
		}
		public OperationResult SaveOrganisationCardTemplates(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveCardTemplates(organisation);
		}
		public OperationResult SaveOrganisationGuardZones(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveGuardZones(organisation);
		}
		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveUsers(organisation);
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SKDDatabaseService.OrganisationTranslator.GetDetails(uid);
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.GetList(filter);
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
		}
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.Save(item);
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
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
				AddSKDJournalMessage(JournalEventNameType.Запрос_информации_об_устройстве, device);
				return ChinaSKDDriver.Processor.GetdeviceInfo(deviceUID);
			}
			return new OperationResult<SKDDeviceInfo>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Синхронизация_времени, device);
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
				AddSKDJournalMessage(JournalEventNameType.Перезапись_всех_карт, device);
				var cardsResult = SKDDatabaseService.CardTranslator.Get(new CardFilter());
				var accessTemplatesResult = SKDDatabaseService.AccessTemplateTranslator.Get(new AccessTemplateFilter());
				if (!cardsResult.HasError && !accessTemplatesResult.HasError)
				{
					return ChinaSKDDriver.Processor.SKDRewriteAllCards(device, cardsResult.Result, accessTemplatesResult.Result);
				}
				else
				{
					return new OperationResult<bool>("Ошибка при получении карт или шаблонов карт");
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
				return ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, doorConfiguration);
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
				if(door.InDevice != null)
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