using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

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
		public OperationResult SaveEmployee(Employee item)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployee(item));
		}
		public OperationResult MarkDeletedEmployee(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployee(uid));
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeeDepartment(uid, departmentUid));
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid positionUid)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployeePosition(uid, positionUid));
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
		public OperationResult SaveDepartment(Department item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartment(item));
		}
		public OperationResult MarkDeletedDepartment(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(uid));
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID));
		}
		public OperationResult SaveDepartmentHRChief(Guid uid, Guid chiefUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentHRChief(uid, chiefUID));
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
		public OperationResult SavePosition(Position item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePosition(item));
		}
		public OperationResult MarkDeletedPosition(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPosition(uid));
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetMinJournalDateTime());
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetFilteredJournalItems(filter));
		}
		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			SafeOperationCall(() => FiresecService.BeginGetFilteredArchive(archiveFilter, archivePortionUID), "BeginGetFilteredArchive");
		}
		public OperationResult<List<JournalEventDescriptionType>> GetDistinctEventDescriptions()
		{
			return SafeContext.Execute<OperationResult<List<JournalEventDescriptionType>>>(() => FiresecService.GetDistinctEventDescriptions());
		}
		public OperationResult<List<JournalEventNameType>> GetDistinctEventNames()
		{
			return SafeContext.Execute<OperationResult<List<JournalEventNameType>>>(() => FiresecService.GetDistinctEventNames());
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
		public OperationResult<bool> AddCard(SKDCard item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.AddCard(item));
		}
		public OperationResult<bool> EditCard(SKDCard item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.EditCard(item));
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.DeleteCardFromEmployee(item, reason));
		}
		public OperationResult MarkDeletedCard(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedCard(uid));
		}
		public OperationResult DeletedCard(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.DeletedCard(uid));
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
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAccessTemplate(item));
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAccessTemplate(uid));
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organisation>>>(() => FiresecService.GetOrganisations(filter));
		}
		public OperationResult SaveOrganisation(OrganisationDetails item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisation(item));
		}
		public OperationResult MarkDeletedOrganisation(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedOrganisation(uid));
		}
		public OperationResult SaveOrganisationDoors(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationDoors(item));
		}
		public OperationResult SaveOrganisationZones(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationZones(item));
		}
		public OperationResult SaveOrganisationGuardZones(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationGuardZones(item));
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationUsers(item));
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<OrganisationDetails>>(() => FiresecService.GetOrganisationDetails(uid));
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationChief(uid, chiefUID));
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
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnType(item));
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumnType(uid));
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
        {
            return SafeContext.Execute<OperationResult<NightSettings>>(() => FiresecService.GetNightSettingsByOrganisation(organisationUID));
        }
        public OperationResult SaveNightSettings(NightSettings nightSettings)
        {
            return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNightSettings(nightSettings));
        }
        #endregion

		#region Devices

		public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() => FiresecService.CancelGKProgress(progressCallbackUID, userName), "CancelSKDProgress");
		}

		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}
		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(device.UID); }, "SKDGetDeviceInfo");
		}

		public OperationResult<bool> SKDSyncronyseTime(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(device.UID); }, "SKDSyncronyseTime");
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

		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteAllTimeSheduleConfiguration(); }, "SKDWriteAllTimeSheduleConfiguration");
		}

		public OperationResult<bool> SKDRewriteAllCards(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRewriteAllCards(device.UID); }, "SKDRewriteAllCards");
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

		public OperationResult<bool> SKDOpenDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public OperationResult<bool> SKDCloseDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
		}

		public OperationResult<bool> SKDOpenDeviceForever(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDeviceForever(device.UID); }, "SKDOpenDeviceForever");
		}

		public OperationResult<bool> SKDCloseDeviceForever(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDeviceForever(device.UID); }, "SKDCloseDeviceForever");
		}

		public OperationResult<bool> SKDOpenZone(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZone(zone.UID); }, "SKDOpenZone");
		}

		public OperationResult<bool> SKDCloseZone(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZone(zone.UID); }, "SKDCloseZone");
		}

		public OperationResult<bool> SKDOpenZoneForever(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZoneForever(zone.UID); }, "SKDOpenZoneForever");
		}

		public OperationResult<bool> SKDCloseZoneForever(SKDZone zone)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZoneForever(zone.UID); }, "SKDCloseZoneForever");
		}

		public OperationResult<bool> SKDOpenDoor(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoor(door.UID); }, "SKDOpenDoor");
		}

		public OperationResult<bool> SKDCloseDoor(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoor(door.UID); }, "SKDCloseDoor");
		}

		public OperationResult<bool> SKDOpenDoorForever(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoorForever(door.UID); }, "SKDOpenDoorForever");
		}

		public OperationResult<bool> SKDCloseDoorForever(SKDDoor door)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoorForever(door.UID); }, "SKDCloseDoorForever");
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
		public OperationResult SavePassCardTemplate(PassCardTemplate item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePassCardTemplate(item));
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPassCardTemplate(uid));
		}
		#endregion

	}
}