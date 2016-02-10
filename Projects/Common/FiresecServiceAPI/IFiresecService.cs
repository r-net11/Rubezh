using FiresecAPI.Journal;
using FiresecAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace FiresecAPI
{
	[ServiceContract]
	public interface IFiresecService : IFiresecServiceSKD
	{
		#region Service

		[OperationContract]
		void CancelSKDProgress(Guid progressCallbackUID, string userName);

		[OperationContract]
		OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew);

		[OperationContract]
		OperationResult<bool> Reconnect(Guid uid, string userName, string password);

		[OperationContract(IsOneWay = true)]
		void Disconnect(Guid uid);

		[OperationContract]
		List<CallbackResult> Poll(Guid uid);

		[OperationContract(IsOneWay = true)]
		void NotifyClientsOnConfigurationChanged();

		[OperationContract]
		SecurityConfiguration GetSecurityConfiguration();

		[OperationContract]
		string Ping();

		[OperationContract]
		List<ServerTask> GetServerTasks();

		[OperationContract]
		OperationResult ResetDB();

		#endregion Service

		#region Journal

		[OperationContract]
		OperationResult<DateTime> GetMinJournalDateTime();

		[OperationContract]
		OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter journalFilter);

		[OperationContract]
		OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID);

		[OperationContract]
		OperationResult<bool> AddJournalItem(JournalItem journalItem);

		#endregion Journal

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

		[OperationContract]
		void SetLocalConfig();

		#endregion Files

		#region Reporting

		[OperationContract]
		OperationResult<List<string>> GetAllReportNames();
		#endregion

		/// <summary>
		/// Проверяет доступность СУБД MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название именованной установки сервера СУБД</param>
		/// <param name="useIntegratedSecurity">Метод аутентификации</param>
		/// <param name="userID">Логин (только для SQL Server аутентификации)</param>
		/// <param name="userPwd">Пароль (только для SQL Server аутентификации)</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		[OperationContract]
		OperationResult<bool> CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID, string userPwd);
	}
}