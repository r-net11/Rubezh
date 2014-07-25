using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public partial interface IFiresecServiceSKD
	{
		#region Employee
		[OperationContract]
		OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter);
		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid uid);
		[OperationContract]
		OperationResult SaveEmployee(Employee item);
		[OperationContract]
		OperationResult MarkDeletedEmployee(Guid uid);
		[OperationContract]
		OperationResult<List<EmployeeTimeTrack>> GetEmployeeTimeTracks(Guid employeeUID, DateTime startDate, DateTime endDate);
		#endregion

		#region Department
		[OperationContract]
		OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter);
		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid uid);
		[OperationContract]
		OperationResult SaveDepartment(Department item);
		[OperationContract]
		OperationResult MarkDeletedDepartment(Guid uid);
		#endregion

		#region Position
		[OperationContract]
		OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter);
		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid uid);
		[OperationContract]
		OperationResult SavePosition(Position item);
		[OperationContract]
		OperationResult MarkDeletedPosition(Guid uid);
		#endregion

		#region Card
		[OperationContract]
		OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter);
		[OperationContract]
		OperationResult AddCard(SKDCard item);
		[OperationContract]
		OperationResult EditCard(SKDCard item);
		[OperationContract]
		OperationResult DeleteCardFromEmployee(SKDCard item, string reason = null);
		[OperationContract]
		OperationResult MarkDeletedCard(Guid uid);
		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card);
		#endregion

		#region AccessTemplate
		[OperationContract]
		OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);
		[OperationContract]
		OperationResult SaveAccessTemplate(AccessTemplate item);
		[OperationContract]
		OperationResult MarkDeletedAccessTemplate(Guid uid);
		#endregion

		#region Organisation
		[OperationContract]
		OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter);
		[OperationContract]
		OperationResult SaveOrganisation(OrganisationDetails Organisation);
		[OperationContract]
		OperationResult MarkDeletedOrganisation(Guid uid);
		[OperationContract]
		OperationResult SaveOrganisationDoors(Organisation organisation);
		[OperationContract]
		OperationResult SaveOrganisationZones(Organisation organisation);
		[OperationContract]
		OperationResult SaveOrganisationCardTemplates(Organisation organisation);
		[OperationContract]
		OperationResult SaveOrganisationGuardZones(Organisation organisation);
		[OperationContract]
		OperationResult SaveOrganisationUsers(Organisation organisation);
		[OperationContract]
		OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid);
		#endregion

		#region Document
		[OperationContract]
		OperationResult<IEnumerable<ShortDocument>> GetDocumentList(DocumentFilter filter);
		[OperationContract]
		OperationResult<Document> GetDocumentDetails(Guid uid);
		[OperationContract]
		OperationResult SaveDocument(Document item);
		[OperationContract]
		OperationResult MarkDeletedDocument(Guid uid);
		#endregion

		#region AdditionalColumnType
		[OperationContract]
		OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter);
		[OperationContract]
		OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid);
		[OperationContract]
		OperationResult SaveAdditionalColumnType(AdditionalColumnType item);
		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnType(Guid uid);
		#endregion

		#region EmployeeReplacement
		[OperationContract]
		OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter);
		[OperationContract]
		OperationResult SaveEmployeeReplacement(EmployeeReplacement item);
		[OperationContract]
		OperationResult MarkDeletedEmployeeReplacement(Guid uid);
		#endregion

		#region DeviceCommands
		[OperationContract]
		OperationResult<SKDStates> SKDGetStates();

		[OperationContract]
		OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSyncronyseTime(Guid deviceUID);

		[OperationContract]
		OperationResult<string> SKDGetPassword(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetPassword(Guid deviceUID, string password);

		[OperationContract]
		OperationResult<bool> SKDResetController(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDRebootController(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDWriteAllTimeSheduleConfiguration();

		[OperationContract]
		OperationResult<bool> SKDRewriteAllCards(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName);

		[OperationContract]
		OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration);

		[OperationContract]
		OperationResult<bool> SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDOpenZone(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDCloseZone(Guid zoneUID);

		[OperationContract]
		OperationResult<bool> SKDOpenDoor(Guid doorUID);

		[OperationContract]
		OperationResult<bool> SKDCloseDoor(Guid doorUID);
		#endregion
	}
}