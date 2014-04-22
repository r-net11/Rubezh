using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using Holiday = FiresecAPI.EmployeeTimeIntervals.Holiday;
using HolidayFilter = FiresecAPI.EmployeeTimeIntervals.HolidayFilter;
using NamedInterval = FiresecAPI.EmployeeTimeIntervals.NamedInterval;
using NamedIntervalFilter = FiresecAPI.EmployeeTimeIntervals.NamedIntervalFilter;
using TimeInterval = FiresecAPI.EmployeeTimeIntervals.TimeInterval;
using TimeIntervalFilter = FiresecAPI.EmployeeTimeIntervals.TimeIntervalFilter;

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
		public OperationResult SaveEmployees(IEnumerable<Employee> items)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployees(items));
		}
		public OperationResult MarkDeletedEmployees(IEnumerable<Guid> uids)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployees(uids));
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
		public OperationResult SaveDepartments(IEnumerable<Department> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartments(items));
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Guid> uids)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartments(uids));
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
		public OperationResult SavePositions(IEnumerable<Position> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePositions(items));
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Guid> uids)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedPositions(uids));
		}
		#endregion

		#region Journal
		public OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDJournalItem>>>(() => FiresecService.GetSKDJournalItems(filter));
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSKDJournalItems(journalItems));
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCards(items));
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedCards(items));
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute(() => FiresecService.SaveCardTemplate(item));
		}
		#endregion

		#region CardZone
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<CardZone>>>(() => FiresecService.GetCardZones(filter));
		}
		public OperationResult SaveCardZoneLinks(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardZones(items));
		}
		public OperationResult MarkDeletedCardZoneLinks(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedCardZones(items));
		}
		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAccessTemplates(items));
		}
		public OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAccessTemplates(items));
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organisation>>>(() => FiresecService.GetOrganisations(filter));
		}
		public OperationResult SaveOrganisations(IEnumerable<Organisation> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisations(items));
		}
		public OperationResult MarkDeletedOrganisations(IEnumerable<Organisation> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedOrganisations(items));
		}
		public OperationResult SaveOrganisationZones(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationZones(item));
		}
		#endregion

		#region Document
		public OperationResult<IEnumerable<ShortDocument>> GetDocumentList(DocumentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortDocument>>>(() => FiresecService.GetDocumentList(filter));
		}
		public OperationResult<Document> GetDocumentDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Document>>(() => FiresecService.GetDocumentDetails(uid));
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDocuments(items));
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Guid> uids)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDocuments(uids));
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}
		public OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnTypes(items));
		}
		public OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedAdditionalColumnTypes(items));
		}
		#endregion

		#region AdditionalColumn
		public OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumn>>>(() => FiresecService.GetAdditionalColumns(filter));
		}
		public OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumns(items));
		}
		#endregion

		#region Photo
		public OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Photo>>>(() => FiresecService.GetPhotos(filter));
		}
		public OperationResult SavePhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePhotos(items));
		}
		#endregion

		#region EmployeeReplacement
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<EmployeeReplacement>>>(() => FiresecService.GetEmployeeReplacements(filter));
		}
		public OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeReplacements(items));
		}
		public OperationResult MarkDeletedEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployeeReplacements(items));
		}
		#endregion

		#region NamedInterval
		public OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<NamedInterval>>>(() => FiresecService.GetNamedIntervals(filter));
		}
		public OperationResult SaveNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveNamedIntervals(items));
		}
		public OperationResult MarkDeletedNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedNamedIntervals(items));
		}
		#endregion

		#region TimeInterval
		public OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<TimeInterval>>>(() => FiresecService.GetTimeIntervals(filter));
		}
		public OperationResult SaveTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveTimeIntervals(items));
		}
		public OperationResult MarkDeletedTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedTimeIntervals(items));
		}
		#endregion

		#region Holiday
		public OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public OperationResult SaveHolidays(IEnumerable<Holiday> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveHolidays(items));
		}
		public OperationResult MarkDeletedHolidays(IEnumerable<Holiday> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHolidays(items));
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