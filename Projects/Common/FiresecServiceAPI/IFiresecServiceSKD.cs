using System;
using System.Collections.Generic;
using System.ServiceModel;

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
		OperationResult SaveEmployees(IEnumerable<Employee> Employees);
		[OperationContract]
		OperationResult MarkDeletedEmployees(IEnumerable<Guid> uids);
		#endregion

		#region Department
		[OperationContract]
		OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter);
		[OperationContract]
		OperationResult<Department> GetDepartmentDetails(Guid uid);
		[OperationContract]
		OperationResult SaveDepartments(IEnumerable<Department> Departments);
		[OperationContract]
		OperationResult MarkDeletedDepartments(IEnumerable<Guid> uids);
		#endregion

		#region Position
		[OperationContract]
		OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter);
		[OperationContract]
		OperationResult<Position> GetPositionDetails(Guid uid);
		[OperationContract]
		OperationResult SavePositions(IEnumerable<Position> Positions);
		[OperationContract]
		OperationResult MarkDeletedPositions(IEnumerable<Guid> uids);
		#endregion

		#region Journal
		[OperationContract]
		OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter);
		[OperationContract]
		OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		#endregion

		#region Card
		[OperationContract]
		OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter);
		[OperationContract]
		OperationResult SaveCards(IEnumerable<SKDCard> items);
		[OperationContract]
		OperationResult MarkDeletedCards(IEnumerable<SKDCard> items);
		[OperationContract]
		OperationResult SaveCardTemplate(SKDCard card);
		#endregion

		#region CardZone
		[OperationContract]
		OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter);
		[OperationContract]
		OperationResult SaveCardZones(IEnumerable<CardZone> items);
		[OperationContract]
		OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items);
		#endregion

		#region AccessTemplate
		[OperationContract]
		OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter);
		[OperationContract]
		OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items);
		[OperationContract]
		OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items);
		#endregion

		#region Organisation
		[OperationContract]
		OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter);
		[OperationContract]
		OperationResult SaveOrganisations(IEnumerable<Organisation> Organisations);
		[OperationContract]
		OperationResult MarkDeletedOrganisations(IEnumerable<Organisation> Organisations);
		[OperationContract]
		OperationResult SaveOrganisationZones(Organisation organization);
		#endregion

		#region Document
		[OperationContract]
		OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter);
		[OperationContract]
		OperationResult SaveDocuments(IEnumerable<Document> items);
		[OperationContract]
		OperationResult MarkDeletedDocuments(IEnumerable<Document> items);
		#endregion

		#region AdditionalColumnType
		[OperationContract]
		OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter);
		[OperationContract]
		OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items);
		[OperationContract]
		OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items);
		#endregion

		#region AdditionalColumn
		[OperationContract]
		OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter);
		[OperationContract]
		OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items);
		#endregion

		#region Photo
		[OperationContract]
		OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter);
		[OperationContract]
		OperationResult SavePhotos(IEnumerable<Photo> items);
		#endregion

		#region EmployeeReplacement
		[OperationContract]
		OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter);
		[OperationContract]
		OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items);
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