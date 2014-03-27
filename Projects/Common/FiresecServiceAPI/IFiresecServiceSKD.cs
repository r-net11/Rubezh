using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecServiceSKD
	{
		#region Get
		[OperationContract]
		OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter);
		[OperationContract]
		OperationResult<EmployeeDetails> GetEmployeeDetails(Guid uid);
		[OperationContract]
		OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter);
		[OperationContract]
		OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter);
		#endregion

		#region Save
		[OperationContract]
		OperationResult SaveEmployees(IEnumerable<Employee> Employees);
		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card);
		[OperationContract]
		OperationResult SaveDepartments(IEnumerable<Department> Departments);
		[OperationContract]
		OperationResult SavePositions(IEnumerable<Position> Positions);
		[OperationContract]
		OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		[OperationContract]
		OperationResult SaveCards(IEnumerable<SKDCard> items);
		[OperationContract]
		OperationResult SaveCardZones(IEnumerable<CardZone> items);
		[OperationContract]
		OperationResult SaveOrganizations(IEnumerable<Organization> Organizations);
		[OperationContract]
		OperationResult SaveOrganizationZones(Organization organization);
		[OperationContract]
		OperationResult SaveDocuments(IEnumerable<Document> items);
		[OperationContract]
		OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items);
		[OperationContract]
		OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items);
		[OperationContract]
		OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items);
		[OperationContract]
		OperationResult SavePhotos(IEnumerable<Photo> items);
		[OperationContract]
		OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items);
		#endregion

		#region MarkDeleted
		[OperationContract]
		OperationResult MarkDeletedEmployees(IEnumerable<Employee> Employees);
		[OperationContract]
		OperationResult MarkDeletedDepartments(IEnumerable<Department> Departments);
		[OperationContract]
		OperationResult MarkDeletedPositions(IEnumerable<Position> Positions);
		[OperationContract]
		OperationResult MarkDeletedCards(IEnumerable<SKDCard> items);
		[OperationContract]
		OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items);
		[OperationContract]
		OperationResult MarkDeletedOrganizations(IEnumerable<Organization> Organizations);
		[OperationContract]
		OperationResult MarkDeletedDocuments(IEnumerable<Document> items);
		[OperationContract]
		OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items);
		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items);
		[OperationContract]
		OperationResult MarkDeletedAdditionalColumns(IEnumerable<AdditionalColumn> items);
		[OperationContract]
		OperationResult MarkDeletedPhotos(IEnumerable<Photo> items);
		[OperationContract]
		OperationResult MarkDeletedEmployeeReplacements(IEnumerable<EmployeeReplacement> items);
		#endregion

		#region DeviceCommands
		[OperationContract]
		OperationResult<SKDStates> SKDGetStates();

		[OperationContract]
		OperationResult<string> SKDGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSyncronyseTime(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDWriteConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName);

		[OperationContract]
		void SKDSetRegimeOpen(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeClose(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeControl(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeConversation(Guid deviceUID);

		[OperationContract]
		void SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		void SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		void SKDAllowReader(Guid deviceUID);

		[OperationContract]
		void SKDDenyReader(Guid deviceUID);
		#endregion
	}
}