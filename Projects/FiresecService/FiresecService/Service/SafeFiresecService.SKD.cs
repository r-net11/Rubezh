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
		public OperationResult SaveEmployee(Employee item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployee(item));
		}
		public OperationResult MarkDeletedEmployee(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedEmployee(uid));
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute<OperationResult<TimeTrackResult>>(() => FiresecService.GetTimeTracks(filter, startDate, endDate));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPosition(uid));
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
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.SaveAccessTemplate(item));
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAccessTemplate(uid));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedOrganisation(uid));
		}
		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationDoors(organisation));
		}
		public OperationResult SaveOrganisationZones(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationZones(organisation));
		}
		public OperationResult SaveOrganisationCardTemplates(Organisation organisation)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationCardTemplates(organisation));
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
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumnType(uid));
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
		public OperationResult<SKDControllerDirectionType> GetDirectionType(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectionType(deviceUID); }, "GetDirectionType");
		}
		public OperationResult<bool> SetDirectionType(Guid deviceUID, SKDControllerDirectionType directionType)
		{
			return SafeOperationCall(() => { return FiresecService.SetDirectionType(deviceUID, directionType); }, "SetDirectionType");
		}
		public OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			return SafeOperationCall(() => { return FiresecService.SetControllerPassword(deviceUID, name, oldPassword, password); }, "SetControllerPassword");
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
	}
}