using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortEmployee>>>(() => FiresecService.GetEmployeeList(filter));
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Employee>>(() => FiresecService.GetEmployeeDetails(uid));
		}
		public OperationResult SaveEmployee(Employee item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployee(item, isNew));
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployee(uid, name));
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<Stream>(() => FiresecService.GetTimeTracksStream(filter, startDate, endDate));
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid, name));
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid positionUid, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeePosition(uid, positionUid, name));
		}
		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreEmployee(uid, name));
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortDepartment>>>(() => FiresecService.GetDepartmentList(filter));
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Department>>(() => FiresecService.GetDepartmentDetails(uid));
		}
		public OperationResult SaveDepartment(Department item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartment(item, isNew));
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(item));
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name));
		}
		public OperationResult RestoreDepartment(ShortDepartment item)
		{
			return SafeContext.Execute(() => FiresecService.RestoreDepartment(item));
		}
		public OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetChildEmployeeUIDs(uid));
		}
		public OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetParentEmployeeUIDs(uid));
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortPosition>>>(() => FiresecService.GetPositionList(filter));
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Position>>(() => FiresecService.GetPositionDetails(uid));
		}
		public OperationResult SavePosition(Position item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePosition(item, isNew));
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPosition(uid, name));
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestorePosition(uid, name));
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute(() => FiresecService.GetMinJournalDateTime());
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetFilteredJournalItems(filter));
		}
		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			SafeOperationCall(() => FiresecService.BeginGetFilteredArchive(archiveFilter, archivePortionUID), "BeginGetFilteredArchive");
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddJournalItem(journalItem));
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetEmployeeCards(employeeUID));
		}

		public OperationResult<bool> ResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo, string doorName, string organisationName)
		{
			return SafeContext.Execute(() => FiresecService.ResetRepeatEnter(cardsToReset, cardNo, doorName, organisationName));
		}

		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddCard(item, employeeName));
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.EditCard(item, employeeName));
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteCardFromEmployee(item, employeeName, reason));
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeContext.Execute(() => FiresecService.DeletedCard(card));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCardTemplate(item));
		}
		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAccessTemplate(item, isNew));
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAccessTemplate(uid, name));
		}
		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreAccessTemplate(uid, name));
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetOrganisations(filter));
		}
		public OperationResult SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveOrganisation(item, isNew));
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedOrganisation(uid, name));
		}
		public OperationResult SaveOrganisationDoors(Organisation item)
		{
			return SafeContext.Execute(() => FiresecService.SaveOrganisationDoors(item));
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationUsers(item));
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<OrganisationDetails>>(() => FiresecService.GetOrganisationDetails(uid));
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationChief(uid, chiefUID, name));
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationHRChief(uid, chiefUID, name));
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreOrganisation(uid, name));
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.IsAnyOrganisationItems(uid));
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortAdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypeList(filter));
		}

		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}

		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<AdditionalColumnType>>(() => FiresecService.GetAdditionalColumnTypeDetails(uid));
		}
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnType(item, isNew));
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name));
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreAdditionalColumnType(uid, name));
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeContext.Execute(() => FiresecService.GetNightSettingsByOrganisation(organisationUID));
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNightSettings(nightSettings));
		}
		#endregion

		#region Devices

		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}

		/// <summary>
		/// Получает информацию о контроллере.
		/// Такую как версия прошивки, сетевые настройки, дата и время.
		/// </summary>
		/// <param name="device">Доменная модель контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(device.UID); }, "SKDGetDeviceInfo");
		}

		/// <summary>
		/// Устанавливает на контроллере текущее системное время
		/// </summary>
		/// <param name="device">Доменная модель контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDSynchronizeTime(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSynchronizeTime(device.UID); }, "SKDSynchronizeTime");
		}

		public OperationResult<bool> SKDResetController(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDResetController(device.UID); }, "SKDResetController");
		}

		public OperationResult<bool> SKDRebootController(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRebootController(device.UID); }, "SKDRebootController");
		}

		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteTimeSheduleConfiguration(device.UID); }, "SKDWriteTimeSheduleConfiguration");
		}

		/// <summary>
		/// Записывает графики доступа на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteAllTimeSheduleConfiguration(); }, "SKDWriteAllTimeSheduleConfiguration");
		}

		public OperationResult<bool> SKDRewriteAllCards(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRewriteAllCards(device.UID); }, "SKDRewriteAllCards");
		}

		/// <summary>
		/// Перезаписывает пропуска на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteCardsOnAllControllers()
		{
			return SafeOperationCall(() => { return FiresecService.RewriteCardsOnAllControllers(); }, "RewriteCardsOnAllControllers");
		}

		public OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(device.UID, fileName); }, "SKDUpdateFirmware");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDoorConfiguration(device.UID); }, "SKDGetDoorConfiguration");
		}

		public OperationResult<bool> SKDSetDoorConfiguration(SKDDevice device, SKDDoorConfiguration doorConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetDoorConfiguration(device.UID, doorConfiguration); }, "SKDSetDoorConfiguration");
		}

		public OperationResult<DoorType> GetControllerDoorType(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerDoorType(device.UID); }, "GetControllerDoorType");
		}

		public OperationResult<bool> SetControllerDoorType(SKDDevice device, DoorType doorType)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerDoorType(device.UID, doorType); }, "SetControllerDoorType");
		}

		public OperationResult<bool> SetControllerPassword(SKDDevice device, string name, string oldPassword, string password)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerPassword(device.UID, name, oldPassword, password); }, "SetControllerPassword");
		}

		public OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerTimeSettings(device.UID); }, "GetControllerTimeSettings");
		}

		public OperationResult<bool> SetControllerTimeSettings(SKDDevice device, SKDControllerTimeSettings controllerTimeSettings)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerTimeSettings(device.UID, controllerTimeSettings); }, "SetControllerTimeSettings");
		}

		public OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerNetworkSettings(device.UID); }, "GetControllerNetworkSettings");
		}

		public OperationResult<bool> SetControllerNetworkSettings(SKDDevice device, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerNetworkSettings(device.UID, controllerNetworkSettings); }, "SetControllerNetworkSettings");
		}

		public OperationResult<bool> SetControllerTimeSchedulesAndLocksPasswords(SKDDevice device, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerTimeSchedulesAndLocksPasswords(device.UID, locksPasswords); }, "SetControllerTimeSchedulesAndLocksPasswords");
		}


		public OperationResult<bool> SKDOpenDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public OperationResult<bool> SKDCloseDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
		}

		public OperationResult<bool> SKDDeviceAccessStateNormal(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateNormal(device.UID); }, "SKDDeviceAccessStateNormal");
		}

		public OperationResult<bool> SKDDeviceAccessStateCloseAlways(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateCloseAlways(device.UID); }, "SKDDeviceAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDDeviceAccessStateOpenAlways(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateOpenAlways(device.UID); }, "SKDDeviceAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearDevicePromptWarning(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearDevicePromptWarning(device.UID); }, "SKDClearDevicePromptWarning");
		}

		public OperationResult<bool> SKDOpenZone(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZone(zone.UID); }, "SKDOpenZone");
		}

		public OperationResult<bool> SKDCloseZone(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZone(zone.UID); }, "SKDCloseZone");
		}

		public OperationResult<bool> SKDZoneAccessStateNormal(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateNormal(zone.UID); }, "SKDZoneAccessStateNormal");
		}

		//public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		//{
		//	SafeOperationCall(() => FiresecService.CancelGKProgress(progressCallbackUID, userName), "CancelSKDProgress");
		//}

		public OperationResult<bool> SKDZoneAccessStateCloseAlways(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateCloseAlways(zone.UID); }, "SKDZoneAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDZoneAccessStateOpenAlways(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateOpenAlways(zone.UID); }, "SKDZoneAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearZonePromptWarning(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearZonePromptWarning(zone.UID); }, "SKDClearZonePromptWarning");
		}

		public OperationResult<bool> SKDOpenDoor(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoor(door.UID); }, "SKDOpenDoor");
		}

		public OperationResult<bool> SKDCloseDoor(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoor(door.UID); }, "SKDCloseDoor");
		}

		public OperationResult<bool> SKDDoorAccessStateNormal(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateNormal(door.UID); }, "SKDDoorAccessStateNormal");
		}

		public OperationResult<bool> SKDDoorAccessStateCloseAlways(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateCloseAlways(door.UID); }, "SKDDoorAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDDoorAccessStateOpenAlways(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateOpenAlways(door.UID); }, "SKDDoorAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearDoorPromptWarning(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearDoorPromptWarning(door.UID); }, "SKDClearDoorPromptWarning");
		}

		public OperationResult<SKDAntiPassBackConfiguration> SKDGetAntiPassBackConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetAntiPassBackConfiguration(device.UID); }, "SKDGetAntiPassBackConfiguration");
		}

		public OperationResult<bool> SKDSetAntiPassBackConfiguration(SKDDevice device, SKDAntiPassBackConfiguration antiPassBackConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetAntiPassBackConfiguration(device.UID, antiPassBackConfiguration); }, "SKDSetAntiPassBackConfiguration");
		}

		public OperationResult<SKDInterlockConfiguration> SKDGetInterlockConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetInterlockConfiguration(device.UID); }, "SKDGetInterlockConfiguration");
		}

		public OperationResult<bool> SKDSetInterlockConfiguration(SKDDevice device, SKDInterlockConfiguration interlockConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetInterlockConfiguration(device.UID, interlockConfiguration); }, "SKDSetInterlockConfiguration");
		}

		public OperationResult<bool> SKDStartSearchDevices()
		{
			return SafeOperationCall(() => { return FiresecService.SKDStartSearchDevices(); }, "SKDStartSearchDevices");
		}

		public OperationResult<bool> SKDStopSearchDevices()
		{
			return SafeOperationCall(() => { return FiresecService.SKDStopSearchDevices(); }, "SKDStopSearchDevices");
		}

		#endregion

		#region PassCardTemplate
		public OperationResult<IEnumerable<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortPassCardTemplate>>>(() => FiresecService.GetPassCardTemplateList(filter));
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<PassCardTemplate>>(() => FiresecService.GetPassCardTemplateDetails(uid));
		}
		public OperationResult SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePassCardTemplate(item, isNew));
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPassCardTemplate(uid, name));
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestorePassCardTemplate(uid, name));
		}
		#endregion

		#region <Пароли замков>

		/// <summary>
		/// Получить список паролей замков на контроллере
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerLocksPasswords(device.UID); }, "GetControllerLocksPasswords");
		}

		/// <summary>
		/// Записать пароли замков на контроллер
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Список паролей замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SetControllerLocksPasswords(SKDDevice device, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerLocksPasswords(device.UID, locksPasswords); }, "SetControllerLocksPasswords");
		}

		/// <summary>
		/// Перезаписывает пароли замков на всех контроллерах
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteControllerLocksPasswordsOnAllControllers()
		{
			return SafeOperationCall(() => { return FiresecService.RewriteControllerLocksPasswordsOnAllControllers(); }, "RewriteControllerLocksPasswordsOnAllControllers");
		}

		#endregion </Пароли замков>

		public OperationResult ResetSKDDatabase()
		{
			return SafeContext.Execute(() => FiresecService.ResetSKDDatabase());
		}

		public OperationResult GenerateEmployeeDays()
		{
			return SafeContext.Execute(() => FiresecService.GenerateEmployeeDays());
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID));
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveJournalCameraUID(journalItemUID, CameraUID));
		}

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportOrganisation(filter));
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ImportOrganisation(filter));
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportOrganisationList(filter));
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ImportOrganisationList(filter));
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportJournal(filter));
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportConfiguration(filter));
		}
		#endregion


		/// <summary>
		/// Выгружает файл на Сервер приложений
		/// </summary>
		/// <param name="attachment">Метаданные выгружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<Guid> UploadFile(Attachment attachment)
		{
			return SafeOperationCall(() => FiresecService.UploadFile(attachment), "UploadFile");
		}

		/// <summary>
		/// Загружает файл с Сервера приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных загружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<Attachment> DownloadFile(Guid attachmentUID)
		{
			return SafeOperationCall(() => FiresecService.DownloadFile(attachmentUID), "DownloadFile");
		}

		/// <summary>
		/// Удаляет файл из хранилища на Сервере приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных удаляемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> RemoveFile(Guid attachmentUID)
		{
			return SafeOperationCall(() => FiresecService.RemoveFile(attachmentUID), "RemoveFile");
		}
	}
}