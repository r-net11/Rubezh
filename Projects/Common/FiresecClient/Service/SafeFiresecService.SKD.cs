using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
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
		public OperationResult<List<EmployeeTimeTrack>> GetEmployeeTimeTracks(Guid uid, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute(() => FiresecService.GetEmployeeTimeTracks(uid, startDate, endDate));
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
			return SafeContext.Execute(() => FiresecService.MarkDeletedPosition(uid));
		}
		#endregion

		#region Journal
		public OperationResult<IEnumerable<JournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<JournalItem>>>(() => FiresecService.GetSKDJournalItems(filter));
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<JournalItem> journalItems)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSKDJournalItems(journalItems));
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult AddCard(SKDCard item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddCard(item));
		}
		public OperationResult EditCard(SKDCard item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.EditCard(item));
		}
		public OperationResult DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteCardFromEmployee(item, reason));
		}
		public OperationResult MarkDeletedCard(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedCard(uid));
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
		public OperationResult SaveAccessTemplate(AccessTemplate item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAccessTemplate(item));
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
		public OperationResult SaveOrganisationCardTemplates(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganisationCardTemplates(item));
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
		public OperationResult SaveDocument(Document item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDocument(item));
		}
		public OperationResult MarkDeletedDocument(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDocument(uid));
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortAdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypeList(filter));
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

		#region EmployeeReplacement
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<EmployeeReplacement>>>(() => FiresecService.GetEmployeeReplacements(filter));
		}
		public OperationResult SaveEmployeeReplacement(EmployeeReplacement item)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeReplacement(item));
		}
		public OperationResult MarkDeletedEmployeeReplacement(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedEmployeeReplacement(uid));
		}
		#endregion

		#region Devices
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

		public OperationResult<string> SKDGetPassword(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetPassword(deviceUID); }, "SKDGetPassword");
		}

		public OperationResult<bool> SKDSetPassword(Guid deviceUID, string password)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetPassword(deviceUID, password); }, "SKDSetPassword");
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

		public OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(device.UID, fileName); }, "SKDUpdateFirmware");
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDoorConfiguration(deviceUID); }, "SKDGetDoorConfiguration");
		}

		public OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSetDoorConfiguration(deviceUID, doorConfiguration); }, "SKDSetDoorConfiguration");
		}

		public OperationResult<bool> SKDOpenDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDOpenDevice(device.UID); }, "SKDOpenDevice");
		}

		public OperationResult<bool> SKDCloseDevice(SKDDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.SKDCloseDevice(device.UID); }, "SKDCloseDevice");
		}

		public void BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter)
		{
			SafeOperationCall(() => FiresecService.BeginGetSKDFilteredArchive(archiveFilter), "BeginGetSKDFilteredArchive");
		}
		#endregion
	}
}