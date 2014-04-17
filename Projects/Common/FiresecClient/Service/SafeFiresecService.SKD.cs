using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using NamedInterval = FiresecAPI.EmployeeTimeIntervals.NamedInterval;
using NamedIntervalFilter = FiresecAPI.EmployeeTimeIntervals.NamedIntervalFilter;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		#region Get
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortEmployee>>>(() => FiresecService.GetEmployeeList(filter));
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Employee>>(() => FiresecService.GetEmployeeDetails(uid));
		}
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Department>>>(() => FiresecService.GetDepartments(filter));
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Position>>>(() => FiresecService.GetPositions(filter));
		}
		public OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDJournalItem>>>(() => FiresecService.GetSKDJournalItems(filter));
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<CardZone>>>(() => FiresecService.GetCardZones(filter));
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Document>>>(() => FiresecService.GetDocuments(filter));
		}
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}
		public OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumn>>>(() => FiresecService.GetAdditionalColumns(filter));
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organization>>>(() => FiresecService.GetOrganizations(filter));
		}
		public OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Photo>>>(() => FiresecService.GetPhotos(filter));
		}
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<EmployeeReplacement>>>(() => FiresecService.GetEmployeeReplacements(filter));
		}
		public OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<NamedInterval>>>(() => FiresecService.GetNamedIntervals(filter));
		}
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> items)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployees(items));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCardTemplate(item));
		}
		public OperationResult SaveDepartments(IEnumerable<Department> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartments(items));
		}
		public OperationResult SavePositions(IEnumerable<Position> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePositions(items));
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSKDJournalItems(journalItems));
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCards(items));
		}
		public OperationResult SaveCardZoneLinks(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardZones(items));
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDocuments(items));
		}
		public OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAccessTemplates(items));
		}
		public OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnTypes(items));
		}
		public OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumns(items));
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganizations(items));
		}
		public OperationResult SaveOrganizationZones(Organization item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganizationZones(item));
		}
		public OperationResult SavePhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePhotos(items));
		}
		public OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeReplacements(items));
		}
		public OperationResult SaveNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNamedIntervals(items));
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Guid> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployees(items));
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Department> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartments(items));
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPositions(items));
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedCards(items));
		}
		public OperationResult MarkDeletedCardZoneLinks(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedCardZones(items));
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDocuments(items));
		}
		public OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAccessTemplates(items));
		}
		public OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumnTypes(items));
		}
		public OperationResult MarkDeletedAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumns(items));
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedOrganizations(items));
		}
		public OperationResult MarkDeletedPhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPhotos(items));
		}
		public OperationResult MarkDeletedEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployeeReplacements(items));
		}
		public OperationResult MarkDeletedNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedNamedIntervals(items));
		}
		#endregion
		
		#region Devices
		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}
		public OperationResult<string> SKDGetDeviceInfo(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(device.UID); }, "SKDGetDeviceInfo");
		}

		public OperationResult<bool> SKDSyncronyseTime(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(device.UID); }, "SKDSyncronyseTime");
		}

		public OperationResult<bool> SKDWriteConfiguration(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteConfiguration(device.UID); }, "SKDWriteConfiguration");
		}

		public OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(device.UID, fileName); }, "SKDUpdateFirmware");
		}

		public OperationResult<bool> SKDWriteAllIdentifiers(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteAllIdentifiers(device.UID); }, "SKDWriteAllIdentifiers");
		}

		public OperationResult<bool> SKDSetRegimeOpen(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetRegimeOpen(device.UID); }, "SKDSetIgnoreRegime");
		}
		public OperationResult<bool> SKDSetRegimeClose(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetRegimeClose(device.UID); }, "SKDSetIgnoreRegime");
		}
		public OperationResult<bool> SKDSetRegimeControl(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetRegimeControl(device.UID); }, "SKDSetRegimeControl");
		}
		public OperationResult<bool> SKDSetRegimeConversation(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetRegimeConversation(device.UID); }, "SKDSetRegimeConversation");
		}

		public OperationResult<bool> SKDOpenDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public OperationResult<bool> SKDCloseDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
		}

		public OperationResult<bool> SKDAllowReader(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDAllowReader(device.UID); }, "SKDAllowReader");
		}

		public OperationResult<bool> SKDDenyReader(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDDenyReader(device.UID); }, "SKDDenyReader");
		}

		public void BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter)
		{
				SafeOperationCall(() => FiresecService.BeginGetSKDFilteredArchive(archiveFilter), "BeginGetSKDFilteredArchive");
		}
		#endregion
	}
}