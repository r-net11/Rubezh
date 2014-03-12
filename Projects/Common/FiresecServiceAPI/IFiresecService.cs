using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
	[ServiceContract]
	public interface IFiresecService : IFiresecServiceSKD, IGKService
	{
		#region Service
		[OperationContract]
		OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew);

		[OperationContract]
		OperationResult<bool> Reconnect(Guid uid, string userName, string password);

		[OperationContract(IsOneWay = true)]
		void Disconnect(Guid uid);

		[OperationContract]
		List<CallbackResult> Poll(Guid uid);

		[OperationContract]
		string Test(string arg);

		[OperationContract(IsOneWay = true)]
		void NotifyClientsOnConfigurationChanged();

		[OperationContract]
		SecurityConfiguration GetSecurityConfiguration();

		[OperationContract]
		string Ping();
		#endregion

		#region Journal
		[OperationContract]
		OperationResult<int> GetJournalLastId();

		[OperationContract]
		OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter);

		[OperationContract]
		OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter);

		[OperationContract]
		void BeginGetFilteredArchive(ArchiveFilter archiveFilter);

		[OperationContract]
		OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions();

		[OperationContract]
		OperationResult<DateTime> GetArchiveStartDate();

		[OperationContract()]
		void AddJournalRecords(List<JournalRecord> journalRecords);
		#endregion

		#region Files
		[OperationContract]
		List<string> GetFileNamesList(string directory);

		[OperationContract]
		Dictionary<string, string> GetDirectoryHash(string directory);

		[OperationContract]
		Stream GetFile(string dirAndFileName);

		[OperationContract]
		Stream GetConfig();

		[OperationContract]
		void SetConfig(Stream stream);
		#endregion

		#region Convertation
		[OperationContract]
		void SetJournal(List<JournalRecord> journalRecords);
		#endregion
	}
}