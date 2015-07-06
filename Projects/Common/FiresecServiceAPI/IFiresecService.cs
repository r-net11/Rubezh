using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Journal;
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
		OperationResult<ServerState> GetServerState();

		[OperationContract]
		List<CallbackResult> Poll(Guid uid);

		[OperationContract]
		string Test(string arg);

		[OperationContract]
		SecurityConfiguration GetSecurityConfiguration();

		[OperationContract]
		string Ping();

		[OperationContract]
		OperationResult ResetDB();
		#endregion

		#region Journal
		[OperationContract]
		OperationResult<DateTime> GetMinJournalDateTime();

		[OperationContract]
		OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter journalFilter);

		[OperationContract]
		OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID);

		[OperationContract]
		OperationResult<bool> AddJournalItem(JournalItem journalItem);
		#endregion

		#region Files
		[OperationContract]
		List<string> GetFileNamesList(string directory);

		[OperationContract]
		Dictionary<string, string> GetDirectoryHash(string directory);

		[OperationContract]
		Stream GetServerAppDataFile(string dirAndFileName);

		[OperationContract]
		Stream GetServerFile(string filePath);

		[OperationContract]
		Stream GetConfig();

		[OperationContract]
		void SetRemoteConfig(Stream stream);

		[OperationContract]
		void SetLocalConfig();
		#endregion
	}
}