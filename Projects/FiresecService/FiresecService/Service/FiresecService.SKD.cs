using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriver;
using FiresecAPI;
using FiresecAPI.Events;
using FiresecAPI.SKD;
using SKDDriver;

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

		#region Journal
		public OperationResult<IEnumerable<JournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SKDDatabaseService.JournalItemTranslator.Get(filter);
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<JournalItem> journalItems)
		{
			return SKDDatabaseService.JournalItemTranslator.Save(journalItems);
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SKDDatabaseService.CardTranslator.Get(filter);
		}
		public OperationResult AddCard(SKDCard item)
		{
			AddSKDMessage(GlobalEventNameEnum.Добавление_карты, null, UserName);
			var accessTemplate = GetAccessTemplate(item.AccessTemplateUID);
			var cardWriter = ChinaSKDDriver.Processor.AddCard(item);
			var pendingResult = SKDDatabaseService.CardTranslator.AddPendingList(item.UID, GetFailedControllerUIDs(cardWriter));
			if (pendingResult.HasError)
				return pendingResult;
			return SKDDatabaseService.CardTranslator.Save(item);
		}
		public OperationResult EditCard(SKDCard item)
		{
			AddSKDMessage(GlobalEventNameEnum.Редактирование_карты, null, UserName);
			var accessTemplate = GetAccessTemplate(item.AccessTemplateUID);
			var cardWriter = ChinaSKDDriver.Processor.EditCard(item);
			var pendingResult = SKDDatabaseService.CardTranslator.EditPendingList(item.UID, GetFailedControllerUIDs(cardWriter));
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
			AddSKDMessage(GlobalEventNameEnum.Удаление_карты, null, UserName);
			var accessTemplate = GetAccessTemplate(item.AccessTemplateUID);
			var cardWriter = ChinaSKDDriver.Processor.DeleteCard(item);
			var pendingResult = SKDDatabaseService.CardTranslator.DeletePendingList(item.UID, GetFailedControllerUIDs(cardWriter));
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

		#region Document
		public OperationResult<IEnumerable<ShortDocument>> GetDocumentList(DocumentFilter filter)
		{
			return SKDDatabaseService.DocumentTranslator.GetList(filter);
		}
		public OperationResult<Document> GetDocumentDetails(Guid uid)
		{
			return SKDDatabaseService.DocumentTranslator.GetSingle(uid);
		}
		public OperationResult SaveDocument(Document item)
		{
			return SKDDatabaseService.DocumentTranslator.Save(item);
		}
		public OperationResult MarkDeletedDocument(Guid uid)
		{
			return SKDDatabaseService.DocumentTranslator.MarkDeleted(uid);
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

		#region EmployeeReplacement
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Get(filter);
		}
		public OperationResult SaveEmployeeReplacement(EmployeeReplacement item)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Save(item);
		}
		public OperationResult MarkDeletedEmployeeReplacement(Guid uid)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Devices
		public OperationResult<SKDStates> SKDGetStates()
		{
			return new OperationResult<SKDStates>() { Result = ChinaSKDDriver.Processor.SKDGetStates() };
		}

		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Запрос_информации_об_устройстве, device, UserName);
				return new OperationResult<SKDDeviceInfo>() { Result = ChinaSKDDriver.Processor.GetdeviceInfo(deviceUID) };
			}
			return new OperationResult<SKDDeviceInfo>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Синхронизация_времени, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SyncronyseTime(deviceUID) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<string> SKDGetPassword(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Запрос_пароля, device, UserName);
				return new OperationResult<string>() { Result = ChinaSKDDriver.Processor.GetPassword(deviceUID) };
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetPassword(Guid deviceUID, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Установка_пароля, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SetPassword(deviceUID, password) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDResetController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Сброс_Контроллера, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.ResetController(deviceUID) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDRebootController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Перезагрузка_Контроллера, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.RebootController(deviceUID) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Запись_графиков_работы, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SKDWriteTimeSheduleConfiguration(deviceUID) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Обновление_ПО_Контроллера, device, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Запрос_конфигурации_двери, device, UserName);
				return new OperationResult<SKDDoorConfiguration>() { Result = ChinaSKDDriver.Processor.GetDoorConfiguration(deviceUID) };
			}
			return new OperationResult<SKDDoorConfiguration>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Запись_конфигурации_двери, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SetDoorConfiguration(deviceUID, doorConfiguration) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDMessage(GlobalEventNameEnum.Команда_на_открытие_двери, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.OpenDoor(deviceUID) };
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
				AddSKDMessage(GlobalEventNameEnum.Команда_на_закрытие_двери, device, UserName);
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.CloseDoor(deviceUID) };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}
		#endregion

		AccessTemplate GetAccessTemplate(Guid? uid)
		{
			var accessTemplateOperationResult = SKDDatabaseService.AccessTemplateTranslator.GetSingle(uid);
			if (!accessTemplateOperationResult.HasError)
				return accessTemplateOperationResult.Result;
			return null;
		}

		IEnumerable<Guid> GetFailedControllerUIDs(CardWriter cardWriter)
		{
			return cardWriter.ControllerCardItems.Where(x => !x.Result).Select(x => x.ControllerDevice.UID);
		}
	}
}