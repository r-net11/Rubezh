using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;

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
			return SafeContext.Execute<OperationResult<TimeTrackResult>>(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
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
		public OperationResult MarkDeletedDepartment(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartment(uid, name));
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartmentChief(uid, chiefUID, name));
		}
		public OperationResult RestoreDepartment(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreDepartment(uid, name));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPosition(uid, name));
		}

		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestorePosition(uid, name));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedCard(uid));
		}
		public OperationResult DeletedCard(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletedCard(uid));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardTemplate(item));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAccessTemplate(uid, name));
		}
		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAccessTemplate(uid, name));
		}
		#endregion

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
		public OperationResult SaveOrganisationZones(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationZones(organisation));
		}
		public OperationResult SaveOrganisationGuardZones(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationGuardZones(organisation));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumnType(uid, name));
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreAdditionalColumnType(uid, name));
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

		#region Device
		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}
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
		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteAllTimeSheduleConfiguration(); }, "SKDWriteAllTimeSheduleConfiguration");
		}
		public OperationResult<bool> SKDRewriteAllCards(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDRewriteAllCards(deviceUID); }, "SKDRewriteAllCards");
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
		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(deviceUID); }, "SKDOpenDevice");
		}
		public OperationResult<bool> SKDCloseDevice(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(deviceUID); }, "SKDCloseDevice");
		}
		public OperationResult<bool> SKDOpenDeviceForever(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDeviceForever(deviceUID); }, "SKDOpenDeviceForever");
		}
		public OperationResult<bool> SKDCloseDeviceForever(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDeviceForever(deviceUID); }, "SKDCloseDeviceForever");
		}
		public OperationResult<bool> SKDOpenZone(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZone(zoneUID); }, "SKDOpenZone");
		}
		public OperationResult<bool> SKDCloseZone(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZone(zoneUID); }, "SKDCloseZone");
		}
		public OperationResult<bool> SKDOpenZoneForever(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenZoneForever(zoneUID); }, "SKDOpenZoneForever");
		}
		public OperationResult<bool> SKDCloseZoneForever(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseZoneForever(zoneUID); }, "SKDCloseZoneForever");
		}
		public OperationResult<bool> SKDOpenDoor(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoor(doorUID); }, "SKDOpenDoor");
		}
		public OperationResult<bool> SKDCloseDoor(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoor(doorUID); }, "SKDCloseDoor");
		}
		public OperationResult<bool> SKDOpenDoorForever(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDoorForever(doorUID); }, "SKDOpenDoorForever");
		}
		public OperationResult<bool> SKDCloseDoorForever(Guid doorUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDoorForever(doorUID); }, "SKDCloseDoorForever");
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
		#endregion
	}
}