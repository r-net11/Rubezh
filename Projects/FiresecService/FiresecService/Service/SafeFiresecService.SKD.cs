using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Common;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecService.Service
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployee(item, isNew));
		}

		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedEmployee(uid, name));
		}

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<OperationResult<TimeTrackResult>>(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
			return result;
		}

		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<Stream>(() => FiresecService.GetTimeTracksStream(filter, startDate, endDate));
			return result;
		}

		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid, name));
		}

		public OperationResult SaveEmployeePosition(Guid uid, Guid positionUid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeePosition(uid, positionUid, name));
		}

		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreEmployee(uid, name));
		}

		#endregion Employee

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

		public OperationResult MarkDeletedDepartment(ShortDepartment department)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(department));
		}

		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name));
		}

		public OperationResult RestoreDepartment(ShortDepartment department)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreDepartment(department));
		}

		public OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetChildEmployeeUIDs(uid));
		}

		public OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Guid>>>(() => FiresecService.GetParentEmployeeUIDs(uid));
		}

		#endregion Department

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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPosition(uid, name));
		}

		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestorePosition(uid, name));
		}

		#endregion Position

		#region Card

		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}

		public OperationResult<bool> ResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo, string doorName, string organisationName)
		{
			return SafeContext.Execute(() => FiresecService.ResetRepeatEnter(cardsToReset, cardNo, doorName, organisationName));
		}

		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetEmployeeCards(employeeUID));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletedCard(card));
		}

		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardTemplate(item));
		}

		#endregion Card

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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAccessTemplate(uid, name));
		}

		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAccessTemplate(uid, name));
		}

		#endregion AccessTemplate

		#region Organisation

		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organisation>>>(() => FiresecService.GetOrganisations(filter));
		}

		public OperationResult SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisation(item, isNew));
		}

		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedOrganisation(uid, name));
		}

		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationDoors(organisation));
		}

		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationUsers(organisation));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreOrganisation(uid, name));
		}

		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.IsAnyOrganisationItems(uid));
		}

		#endregion Organisation

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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name));
		}

		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAdditionalColumnType(uid, name));
		}

		#endregion AdditionalColumnType

		#region NightSettings

		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<NightSettings>>(() => FiresecService.GetNightSettingsByOrganisation(organisationUID));
		}

		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNightSettings(nightSettings));
		}

		#endregion NightSettings

		#region Device

		public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() => FiresecService.CancelSKDProgress(progressCallbackUID, userName), "CancelSKDProgress");
		}

		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}

		/// <summary>
		/// Получает информацию о контроллере.
		/// Такую как версия прошивки, сетевые настройки, дата и время.
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(deviceUID); }, "SKDGetDeviceInfo");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(deviceUID); }, "SKDSyncronyseTime");
		}

		public OperationResult<bool> SKDResetController(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDResetController(deviceUID); }, "SKDResetController");
		}

		public OperationResult<bool> SKDRebootController(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRebootController(deviceUID); }, "SKDRebootController");
		}

		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteTimeSheduleConfiguration(deviceUID); }, "SKDWriteTimeSheduleConfiguration");
		}

		/// <summary>
		/// Записывает графики доступа на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteAllTimeSheduleConfiguration(); }, "SKDWriteAllTimeSheduleConfiguration");
		}

		public OperationResult<bool> SKDRewriteAllCards(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRewriteAllCards(deviceUID); }, "SKDRewriteAllCards");
		}

		/// <summary>
		/// Перезаписывает пропуска на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteCardsOnAllControllers()
		{
			return SafeOperationCall(() => { return FiresecService.RewriteCardsOnAllControllers(); }, "RewriteCardsOnAllControllers");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(deviceUID, fileName); }, "SKDUpdateFirmware");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDoorConfiguration(deviceUID); }, "SKDGetDoorConfiguration");
		}

		public OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetDoorConfiguration(deviceUID, doorConfiguration); }, "SKDSetDoorConfiguration");
		}

		public OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerDoorType(deviceUID); }, "GetControllerDoorType");
		}

		public OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerDoorType(deviceUID, doorType); }, "SetControllerDoorType");
		}

		public OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerPassword(deviceUID, name, oldPassword, password); }, "SetControllerPassword");
		}

		public OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerTimeSettings(deviceUID); }, "GetControllerTimeSettings");
		}

		public OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerTimeSettings(deviceUID, controllerTimeSettings); }, "SetControllerTimeSettings");
		}

		public OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerNetworkSettings(deviceUID); }, "GetControllerNetworkSettings");
		}

		public OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerNetworkSettings(deviceUID, controllerNetworkSettings); }, "SetControllerNetworkSettings");
		}

		/// <summary>
		/// Записывает на контроллер графики доступа и пароли замков
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Пароли замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SetControllerTimeSchedulesAndLocksPasswords(Guid deviceUID, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerTimeSchedulesAndLocksPasswords(deviceUID, locksPasswords); }, "SetControllerTimeSchedulesAndLocksPasswords");
		}

		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(deviceUID); }, "SKDOpenDevice");
		}

		public OperationResult<bool> SKDCloseDevice(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(deviceUID); }, "SKDCloseDevice");
		}

		public OperationResult<bool> SKDDeviceAccessStateNormal(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateNormal(deviceUID); }, "SKDDeviceAccessStateNormal");
		}

		public OperationResult<bool> SKDDeviceAccessStateCloseAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateCloseAlways(deviceUID); }, "SKDDeviceAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDDeviceAccessStateOpenAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDeviceAccessStateOpenAlways(deviceUID); }, "SKDDeviceAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearDevicePromptWarning(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearDevicePromptWarning(deviceUID); }, "SKDClearDevicePromptWarning");
		}

		public OperationResult<bool> SKDOpenZone(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZone(zoneUID); }, "SKDOpenZone");
		}

		public OperationResult<bool> SKDCloseZone(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZone(zoneUID); }, "SKDCloseZone");
		}

		public OperationResult<bool> SKDZoneAccessStateNormal(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateNormal(deviceUID); }, "SKDZoneAccessStateNormal");
		}

		public OperationResult<bool> SKDZoneAccessStateCloseAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateCloseAlways(deviceUID); }, "SKDZoneAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDZoneAccessStateOpenAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDZoneAccessStateOpenAlways(deviceUID); }, "SKDZoneAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearZonePromptWarning(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearZonePromptWarning(zoneUID); }, "SKDClearZonePromptWarning");
		}

		public OperationResult<bool> SKDOpenDoor(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoor(doorUID); }, "SKDOpenDoor");
		}

		public OperationResult<bool> SKDCloseDoor(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoor(doorUID); }, "SKDCloseDoor");
		}

		public OperationResult<bool> SKDDoorAccessStateNormal(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateNormal(deviceUID); }, "SKDDoorAccessStateNormal");
		}

		public OperationResult<bool> SKDDoorAccessStateCloseAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateCloseAlways(deviceUID); }, "SKDDoorAccessStateCloseAlways");
		}

		public OperationResult<bool> SKDDoorAccessStateOpenAlways(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDoorAccessStateOpenAlways(deviceUID); }, "SKDDoorAccessStateOpenAlways");
		}

		public OperationResult<bool> SKDClearDoorPromptWarning(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDClearDoorPromptWarning(doorUID); }, "SKDClearDoorPromptWarning");
		}

		public OperationResult<SKDAntiPassBackConfiguration> SKDGetAntiPassBackConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetAntiPassBackConfiguration(deviceUID); }, "SKDGetAntiPassBackConfiguration");
		}

		public OperationResult<bool> SKDSetAntiPassBackConfiguration(Guid deviceUID, SKDAntiPassBackConfiguration antiPassBackConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetAntiPassBackConfiguration(deviceUID, antiPassBackConfiguration); }, "SKDSetAntiPassBackConfiguration");
		}

		public OperationResult<SKDInterlockConfiguration> SKDGetInterlockConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetInterlockConfiguration(deviceUID); }, "SKDGetInterlockConfiguration");
		}

		public OperationResult<bool> SKDSetInterlockConfiguration(Guid deviceUID, SKDInterlockConfiguration interlockConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetInterlockConfiguration(deviceUID, interlockConfiguration); }, "SKDSetInterlockConfiguration");
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPassCardTemplate(uid, name));
		}

		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestorePassCardTemplate(uid, name));
		}

		#endregion PassCardTemplate

		#region <Пароли замков>

		/// <summary>
		/// Получить список паролей замков на контроллере
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(Guid deviceUid)
		{
			return SafeOperationCall(() => { return FiresecService.GetControllerLocksPasswords(deviceUid); }, "GetControllerLocksPasswords");
		}

		/// <summary>
		/// Записать пароли замков на контроллер
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Список паролей замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SetControllerLocksPasswords(Guid deviceUid, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerLocksPasswords(deviceUid, locksPasswords); }, "SetControllerLocksPasswords");
		}

		/// <summary>
		/// Перезаписывает пароли замков на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteControllerLocksPasswordsOnAllControllers()
		{
			return SafeOperationCall(() => { return FiresecService.RewriteControllerLocksPasswordsOnAllControllers(); }, "RewriteControllerLocksPasswordsOnAllControllers");
		}

		#endregion </Пароли замков>

		#region Reporting

		public OperationResult<bool> SaveReportFilter(SKDReportFilter filter, User user)
		{
			return SafeContext.Execute(() => FiresecService.SaveReportFilter(filter, user));
		}

		public OperationResult<bool> RemoveReportFilter(SKDReportFilter filter, User user)
		{
			return SafeContext.Execute(() => FiresecService.RemoveReportFilter(filter, user));
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersByType(User user, ReportType type)
		{
			return SafeContext.Execute(() => FiresecService.GetReportFiltersByType(user, type));
		}

		public OperationResult<List<SKDReportFilter>> GetAllFilters()
		{
			return SafeContext.Execute(() => FiresecService.GetAllFilters());
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersForUser(User user)
		{
			return SafeContext.Execute(() => FiresecService.GetReportFiltersForUser(user));
		}
		#endregion

		public OperationResult ResetSKDDatabase()
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ResetSKDDatabase());
		}

		public OperationResult GenerateEmployeeDays()
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.GenerateEmployeeDays());
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportOrganisation(filter));
		}

		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ImportOrganisation(filter));
		}

		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportOrganisationList(filter));
		}

		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ImportOrganisationList(filter));
		}

		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportJournal(filter));
		}

		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.ExportConfiguration(filter));
		}

		public OperationResult ExportReport(ReportExportFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.ExportReport(filter));
		}

		#endregion Export

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