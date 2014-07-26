using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriver;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver;
using FiresecClient;

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
			return SKDDatabaseService.EmployeeTranslator.MarkDeleted(uid);
		}
		public OperationResult<List<EmployeeTimeTrack>> GetEmployeeTimeTracks(Guid employeeUID, DateTime startDate, DateTime endDate)
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
		public OperationResult AddCard(SKDCard item)
		{
			AddSKDJournalMessage(JournalEventNameType.Добавление_карты);
			var accessTemplate = GetAccessTemplate(item.AccessTemplateUID);
			var cardWriter = ChinaSKDDriver.Processor.AddCard(item, accessTemplate);
			var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);
			var pendingResult = SKDDatabaseService.CardTranslator.AddPendingList(item.UID, failedControllerUIDs);
			var saveResult = SKDDatabaseService.CardTranslator.Save(item);

			if (pendingResult.HasError)
				return pendingResult;
			return saveResult;
		}
		public OperationResult EditCard(SKDCard item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_карты);
			var accessTemplate = GetAccessTemplate(item.AccessTemplateUID);

			OperationResult pendingResult;
			var operationResult = SKDDatabaseService.CardTranslator.Get(new CardFilter() { FirstNos = item.Number, LastNos = item.Number });
			var oldCard = operationResult.Result.FirstOrDefault();
			if (oldCard != null)
			{
				var oldAccessTemplate = GetAccessTemplate(oldCard.AccessTemplateUID);

				var cardWriter = ChinaSKDDriver.Processor.EditCard(oldCard, oldAccessTemplate, item, accessTemplate);
				pendingResult = SKDDatabaseService.CardTranslator.EditPendingList(item.UID, GetFailedControllerUIDs(cardWriter));
			}
			else
			{
				pendingResult = new OperationResult("Не найдена предидущая карта");
			}

			if (pendingResult.HasError)
				return pendingResult;
			return SKDDatabaseService.CardTranslator.Save(item);
		}
		public OperationResult DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			item.AccessTemplateUID = null;
			item.CardDoors = new List<CardDoor>();
			item.CardTemplateUID = null;
			item.EmployeeName = null;
			item.HolderUID = null;
			item.IsInStopList = true;
			item.StopReason = reason;
			item.StartDate = DateTime.Now;
			item.EndDate = DateTime.Now;
			AddSKDJournalMessage(JournalEventNameType.Удаление_карты);

			OperationResult pendingResult;
			var operationResult = SKDDatabaseService.CardTranslator.Get(new CardFilter() { FirstNos = item.Number, LastNos = item.Number });
			if (!operationResult.HasError && operationResult.Result != null)
			{
				var oldCard = operationResult.Result.FirstOrDefault();
				if (oldCard != null)
				{
					var accessTemplate = GetAccessTemplate(oldCard.AccessTemplateUID);
					var cardWriter = ChinaSKDDriver.Processor.DeleteCard(oldCard, accessTemplate);
					pendingResult = SKDDatabaseService.CardTranslator.DeletePendingList(oldCard.UID, GetFailedControllerUIDs(cardWriter));
					if (pendingResult.HasError)
						return pendingResult;
				}
				else
				{
					pendingResult = new OperationResult("Не найдена предидущая карта");
				}
			}
			else
			{
				pendingResult = new OperationResult("Не найдена предидущая карта");
			}

			if (pendingResult.HasError)
				return pendingResult;
			return SKDDatabaseService.CardTranslator.Save(item);
		}

		public OperationResult MarkDeletedCard(Guid uid)
		{
			return SKDDatabaseService.CardTranslator.MarkDeleted(uid);
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			return SKDDatabaseService.CardTranslator.SaveTemplate(card);
		}

		AccessTemplate GetAccessTemplate(Guid? uid)
		{
			var accessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(uid);
			if (!accessTemplateOperationResult.HasError)
				return accessTemplateOperationResult.Result;
			return null;
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
		public OperationResult SaveAccessTemplate(AccessTemplate item)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Save(item);
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			return SKDDatabaseService.AccessTemplateTranslator.MarkDeleted(uid);
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

		public OperationResult<string> SKDGetPassword(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_пароля, device);
				return ChinaSKDDriver.Processor.GetPassword(deviceUID);
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetPassword(Guid deviceUID, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Установка_пароля, device);
				return ChinaSKDDriver.Processor.SetPassword(deviceUID, password);
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

		public OperationResult<bool> SKDWriteAllTimeSheduleConfiguration()
		{
			var errors = "";
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					AddSKDJournalMessage(JournalEventNameType.Запись_графиков_работы, device);
					var result = ChinaSKDDriver.Processor.SKDWriteTimeSheduleConfiguration(device.UID);
					if (result.HasError)
						errors += result.Error + " (" + device.Name + ")\n";
				}
			}
			if (string.IsNullOrEmpty(errors))
				return new OperationResult<bool>() { Result = true };
			else return new OperationResult<bool>(errors);
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
		#endregion
	}
}