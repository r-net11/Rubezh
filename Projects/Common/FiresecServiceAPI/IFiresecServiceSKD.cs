using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.Printing;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;

namespace StrazhAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public partial interface IFiresecServiceSKD
	{
		#region Employee

		[OperationContract]
		OperationResult<IEnumerable<Employee>> GetFullEmployeeData(EmployeeFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter);

		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid uid);

		[OperationContract]
		OperationResult SaveEmployee(Employee item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedEmployee(Guid uid, string name);

		[OperationContract]
		OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate);

		[OperationContract]
		OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name);

		[OperationContract]
		OperationResult SaveEmployeePosition(Guid uid, Guid positionUid, string name);

		[OperationContract]
		OperationResult RestoreEmployee(Guid uid, string name);

		#endregion Employee

		#region Department

		[OperationContract]
		OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter);

		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid uid);

		[OperationContract]
		OperationResult SaveDepartment(Department item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedDepartment(ShortDepartment department);

		[OperationContract]
		OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name);

		[OperationContract]
		OperationResult RestoreDepartment(ShortDepartment department);

		[OperationContract]
		OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid);

		#endregion Department

		#region Position

		[OperationContract]
		OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter);

		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid uid);

		[OperationContract]
		OperationResult SavePosition(Position item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPosition(Guid uid, string name);

		[OperationContract]
		OperationResult RestorePosition(Guid uid, string name);

		#endregion Position

		#region Card

		[OperationContract]
		OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID);

		[OperationContract]
		OperationResult<bool> ResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo, string doorName, string organisationName);

		[OperationContract]
		OperationResult<bool> AddCard(SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> EditCard(SKDCard item, string employeeName);

		[OperationContract]
		OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null);

		[OperationContract]
		OperationResult DeletedCard(SKDCard card);

		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card);

		#endregion Card

		#region AccessTemplate

		[OperationContract]
		OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);

		[OperationContract]
		OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAccessTemplate(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreAccessTemplate(Guid uid, string name);

		#endregion AccessTemplate

		#region Organisation

		[OperationContract]
		OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter);

		[OperationContract]
		OperationResult SaveOrganisation(OrganisationDetails organisation, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedOrganisation(Guid uid, string name);

		[OperationContract]
		OperationResult SaveOrganisationDoors(Organisation organisation);

		[OperationContract]
		OperationResult SaveOrganisationUsers(Organisation organisation);

		[OperationContract]
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid);

		[OperationContract]
		OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID, string name);

		[OperationContract]
		OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name);

		[OperationContract]
		OperationResult RestoreOrganisation(Guid uid, string name);

		[OperationContract]
		OperationResult<bool> IsAnyOrganisationItems(Guid uid);

		#endregion Organisation

		#region AdditionalColumnType

		[OperationContract]
		OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter);

		[OperationContract]
		OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid);

		[OperationContract]
		OperationResult SaveAdditionalColumnType(AdditionalColumnType item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreAdditionalColumnType(Guid uid, string name);

		#endregion AdditionalColumnType

		#region DeviceCommands

		[OperationContract]
		OperationResult<SKDStates> SKDGetStates();

		[OperationContract]
		OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSynchronizeTime(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDResetController(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDRebootController(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration();

		[OperationContract]
		OperationResult<bool> SKDRewriteAllCards(Guid deviceUID);

		/// <summary>
		/// Перезаписывает пропуска на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<List<Guid>> RewriteCardsOnAllControllers();

		[OperationContract]
		OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName);

		[OperationContract]
		OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration);

		[OperationContract]
		OperationResult<DoorType> GetControllerDoorType(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType);

		[OperationContract]
		OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password);

		[OperationContract]
		OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings);

		[OperationContract]
		OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings);

		/// <summary>
		/// Записывает на контроллер графики доступа и пароли замков
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Пароли замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<bool> SetControllerTimeSchedulesAndLocksPasswords(Guid deviceUID, IEnumerable<SKDLocksPassword> locksPasswords);

		#region <Пароли замков>

		/// <summary>
		/// Получить список паролей замков на контроллере
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(Guid deviceUid);

		/// <summary>
		/// Записать пароли замков на контроллер
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Список паролей замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<bool> SetControllerLocksPasswords(Guid deviceUid, IEnumerable<SKDLocksPassword> locksPasswords);

		/// <summary>
		/// Перезаписывает пароли замков на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<List<Guid>> RewriteControllerLocksPasswordsOnAllControllers();

		#endregion </Пароли замков>

		#region <Замок>

		[OperationContract]
		OperationResult<bool> SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDDeviceAccessStateNormal(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDDeviceAccessStateCloseAlways(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDDeviceAccessStateOpenAlways(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDClearDevicePromptWarning(Guid deviceUID);

		#endregion </Замок>

		#region <Зона>

		[OperationContract]
		OperationResult<bool> SKDOpenZone(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDCloseZone(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDZoneAccessStateNormal(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDZoneAccessStateCloseAlways(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDZoneAccessStateOpenAlways(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDClearZonePromptWarning(Guid zoneUID);

		#endregion </Зона>

		#region <Точка доступа>

		[OperationContract]
		OperationResult<bool> SKDOpenDoor(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDCloseDoor(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDDoorAccessStateNormal(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDDoorAccessStateCloseAlways(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDDoorAccessStateOpenAlways(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDClearDoorPromptWarning(Guid doorUID);

		#endregion </Точка доступа>

		[OperationContract]
		OperationResult<SKDAntiPassBackConfiguration> SKDGetAntiPassBackConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetAntiPassBackConfiguration(Guid deviceUID, SKDAntiPassBackConfiguration antiPassBackConfiguration);

		[OperationContract]
		OperationResult<SKDInterlockConfiguration> SKDGetInterlockConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetInterlockConfiguration(Guid deviceUID, SKDInterlockConfiguration interlockConfiguration);

		[OperationContract]
		OperationResult<bool> SKDStartSearchDevices();
		[OperationContract]
		OperationResult<bool> SKDStopSearchDevices();

		#endregion

		#region NightSettings

		[OperationContract]
		OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID);

		[OperationContract]
		OperationResult SaveNightSettings(NightSettings nightSettings);

		#endregion NightSettings

		#region PassCardTemplate

		[OperationContract]
		OperationResult<IEnumerable<Tuple<Guid, string>>> GetTemplateNames(Guid organisationId);

		[OperationContract]
		OperationResult<IEnumerable<PassCardTemplate>> GetFullPassCardTemplateList(PassCardTemplateFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter);

		[OperationContract]
		OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid);

		[OperationContract]
		[ServiceKnownType(typeof(PassCardTemplateSide))]
		[ServiceKnownType(typeof(PassCardTemplate))]
		OperationResult SavePassCardTemplate(PassCardTemplate item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedPassCardTemplate(Guid uid, string name);

		[OperationContract]
		OperationResult RestorePassCardTemplate(Guid uid, string name);

		#endregion PassCardTemplate

		[OperationContract]
		OperationResult ResetSKDDatabase();

		[OperationContract]
		OperationResult SaveJournalVideoUID(Guid journaItemUID, Guid videoUID, Guid cameraUID);

		[OperationContract]
		OperationResult SaveJournalCameraUID(Guid journaItemUID, Guid cameraUID);

		#region Export

		[OperationContract]
		OperationResult ExportOrganisation(ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisation(ImportFilter filter);

		[OperationContract]
		OperationResult ExportOrganisationList(ExportFilter filter);

		[OperationContract]
		OperationResult ImportOrganisationList(ImportFilter filter);

		[OperationContract]
		OperationResult ExportJournal(JournalExportFilter filter);

		[OperationContract]
		OperationResult ExportConfiguration(ConfigurationExportFilter filter);

		#endregion Export

		#region Reporting

		[OperationContract]
		[ServiceKnownType(typeof(SKDReportFilter))]
		[ServiceKnownType(typeof(CardsReportFilter))]
		[ServiceKnownType(typeof(DepartmentsReportFilter))]
		[ServiceKnownType(typeof(DisciplineReportFilter))]
		[ServiceKnownType(typeof(DocumentsReportFilter))]
		[ServiceKnownType(typeof(DoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeAccessReportFilter))]
		[ServiceKnownType(typeof(EmployeeDoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeReportFilter))]
		[ServiceKnownType(typeof(EmployeeRootReportFilter))]
		[ServiceKnownType(typeof(EmployeeZonesReportFilter))]
		[ServiceKnownType(typeof(EventsReportFilter))]
		[ServiceKnownType(typeof(PositionsReportFilter))]
		[ServiceKnownType(typeof(SchedulesReportFilter))]
		[ServiceKnownType(typeof(WorkingTimeReportFilter))]
		OperationResult<bool> SaveReportFilter(SKDReportFilter filter, User user);

		[OperationContract]
		OperationResult<bool> RemoveReportFilter(SKDReportFilter filter, User user);

		[OperationContract]
		[ServiceKnownType(typeof(SKDReportFilter))]
		[ServiceKnownType(typeof(CardsReportFilter))]
		[ServiceKnownType(typeof(DepartmentsReportFilter))]
		[ServiceKnownType(typeof(DisciplineReportFilter))]
		[ServiceKnownType(typeof(DocumentsReportFilter))]
		[ServiceKnownType(typeof(DoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeAccessReportFilter))]
		[ServiceKnownType(typeof(EmployeeDoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeReportFilter))]
		[ServiceKnownType(typeof(EmployeeRootReportFilter))]
		[ServiceKnownType(typeof(EmployeeZonesReportFilter))]
		[ServiceKnownType(typeof(EventsReportFilter))]
		[ServiceKnownType(typeof(PositionsReportFilter))]
		[ServiceKnownType(typeof(SchedulesReportFilter))]
		[ServiceKnownType(typeof(WorkingTimeReportFilter))]
		OperationResult<List<SKDReportFilter>> GetReportFiltersByType(User user, ReportType type);
		[OperationContract]
		[ServiceKnownType(typeof(SKDReportFilter))]
		[ServiceKnownType(typeof(CardsReportFilter))]
		[ServiceKnownType(typeof(DepartmentsReportFilter))]
		[ServiceKnownType(typeof(DisciplineReportFilter))]
		[ServiceKnownType(typeof(DocumentsReportFilter))]
		[ServiceKnownType(typeof(DoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeAccessReportFilter))]
		[ServiceKnownType(typeof(EmployeeDoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeReportFilter))]
		[ServiceKnownType(typeof(EmployeeRootReportFilter))]
		[ServiceKnownType(typeof(EmployeeZonesReportFilter))]
		[ServiceKnownType(typeof(EventsReportFilter))]
		[ServiceKnownType(typeof(PositionsReportFilter))]
		[ServiceKnownType(typeof(SchedulesReportFilter))]
		[ServiceKnownType(typeof(WorkingTimeReportFilter))]
		OperationResult<List<SKDReportFilter>> GetReportFiltersForUser(User user);

		[OperationContract]
		[ServiceKnownType(typeof(SKDReportFilter))]
		[ServiceKnownType(typeof(CardsReportFilter))]
		[ServiceKnownType(typeof(DepartmentsReportFilter))]
		[ServiceKnownType(typeof(DisciplineReportFilter))]
		[ServiceKnownType(typeof(DocumentsReportFilter))]
		[ServiceKnownType(typeof(DoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeAccessReportFilter))]
		[ServiceKnownType(typeof(EmployeeDoorsReportFilter))]
		[ServiceKnownType(typeof(EmployeeReportFilter))]
		[ServiceKnownType(typeof(EmployeeRootReportFilter))]
		[ServiceKnownType(typeof(EmployeeZonesReportFilter))]
		[ServiceKnownType(typeof(EventsReportFilter))]
		[ServiceKnownType(typeof(PositionsReportFilter))]
		[ServiceKnownType(typeof(SchedulesReportFilter))]
		[ServiceKnownType(typeof(WorkingTimeReportFilter))]
		OperationResult<List<SKDReportFilter>> GetAllFilters();
		#endregion

		[OperationContract]
		OperationResult<bool> UpdatePassCardOriginalImage(Guid? id, Attachment attachment);

		/// <summary>
		/// Загружает все файлы изображений подложек для шаблона пропуска.
		/// </summary>
		/// <returns>Коллекция, содержащая данные об изображениях.</returns>
		[OperationContract]
		OperationResult<List<Attachment>> UploadPassCardImages();

		/// <summary>
		/// Выгружает файл на Сервер приложений
		/// </summary>
		/// <param name="attachment">Метаданные выгружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<Guid> UploadFile(Attachment attachment);

		/// <summary>
		/// Загружает файл с Сервера приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных загружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<Attachment> DownloadFile(Guid attachmentUID);

		/// <summary>
		/// Удаляет файл из хранилища на Сервере приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных удаляемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<bool> RemoveFile(Guid attachmentUID);
	}
}