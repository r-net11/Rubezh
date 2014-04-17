using System;
using System.Collections.Generic;
using System.ServiceModel;
using EmployeeNamedInterval = FiresecAPI.EmployeeTimeIntervals.NamedInterval;
using EmployeeNamedIntervalFilter = FiresecAPI.EmployeeTimeIntervals.NamedIntervalFilter;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecServiceSKD
	{
		#region Get
		[OperationContract]
		OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter);
		[OperationContract]
		OperationResult<Employee> GetEmployeeDetails(Guid uid);
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
		[OperationContract]
		OperationResult<IEnumerable<EmployeeNamedInterval>> GetNamedIntervals(EmployeeNamedIntervalFilter filter);
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
		[OperationContract]
		OperationResult SaveNamedIntervals(IEnumerable<EmployeeNamedInterval> items);
		#endregion

		#region MarkDeleted
		[OperationContract]
		OperationResult MarkDeletedEmployees(IEnumerable<Guid> uids);
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
		[OperationContract]
		OperationResult MarkDeletedNamedIntervals(IEnumerable<EmployeeNamedInterval> items);
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
		OperationResult<bool> SKDWriteAllIdentifiers(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetRegimeOpen(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetRegimeClose(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetRegimeControl(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSetRegimeConversation(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDAllowReader(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDDenyReader(Guid deviceUID);

		[OperationContract]
		void BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter);
		#endregion
	}
}