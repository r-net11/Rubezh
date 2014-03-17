using System;
using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
		}

		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeOperationCall(() => { return FiresecService.Poll(uid); }, "Poll");
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

		public T GetConfiguration<T>(string filename)
			where T : VersionedConfiguration, new()
		{
			var config = new T();
			return SafeOperationCall(() => config, filename);
		}

		public OperationResult<int> GetJournalLastId()
		{
			return SafeOperationCall(() => { return FiresecService.GetJournalLastId(); }, "GetJournalLastId");
		}

		public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
		}

		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); }, "GetArchiveStartDate");
		}

		public void AddJournalRecords(List<JournalRecord> journalRecords)
		{
			AddTask(() =>
				{
					SafeOperationCall(() => { FiresecService.AddJournalRecords(journalRecords); }, "AddJournalRecords");
				});
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); }, "GetFile");
		}

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetConfig(stream); }, "SetConfig");
		}

		public void SetJournal(List<JournalRecord> journalRecords)
		{
			SafeOperationCall(() => { FiresecService.SetJournal(journalRecords); }, "SetJournal");
		}

		public string Test(string arg)
		{
			return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
		}

		#region SKD
		#region Get
		public OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter)
		{
			return SafeContext.Execute < OperationResult<IEnumerable<Employee>>>(() => FiresecService.GetEmployees(filter));
		}
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return SafeContext.Execute < OperationResult<IEnumerable<Department>>>(() => FiresecService.GetDepartments(filter));
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Position>>>(() => FiresecService.GetPositions(filter));
		}
		public OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute < OperationResult < IEnumerable < SKDJournalItem >>> (() => FiresecService.GetSKDJournalItems(filter));
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
		public OperationResult<IEnumerable<GUD>> GetGUDs(GUDFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<GUD>>>(() => FiresecService.GetGUDs(filter));
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
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> items)
		{
			return SafeContext.Execute(() => FiresecService.SaveEmployees(items));
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
		public OperationResult SaveGUDs(IEnumerable<GUD> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveGUDs(items));
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
		public OperationResult SavePhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePhotos(items));
		}
		public OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeReplacements(items));
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Employee> items)
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
		public OperationResult MarkDeletedGUDs(IEnumerable<GUD> items)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedGUDs(items));
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
		#endregion

		public OperationResult RemoveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveJournalItems(journalItems));
		}

		#endregion

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}
	}
}