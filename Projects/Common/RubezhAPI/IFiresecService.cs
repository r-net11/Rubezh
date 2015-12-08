using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IFiresecService : IFiresecServiceSKD, IGKService, IFiresecServiceAutomation
	{
		#region Service
		/// <summary>
		///  Соединение с сервисом
		/// </summary>
		/// <param name="uid">Уникальный идентификатор клиента</param>
		/// <param name="clientCredentials">Данные подключаемого клиента</param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials);

		/// <summary>
		/// Отсоединение от сервиса
		/// </summary>
		/// <param name="uid">Идентификатор клиента</param>
		[OperationContract(IsOneWay = true)]
		void Disconnect(Guid uid);

		[OperationContract]
		OperationResult<ServerState> GetServerState(Guid clientUID);

		/// <summary>
		/// Поллинг сервера с запросом результатов изменения
		/// Метод реализует концепцию лонг-пол. Т.е. возвращает результат сразу, если есть изменения. Если изменений нет, то он блокируется либо до истечения таймаута в несколько минут, либо до изменения состояния объектов или появлении нового события. Поллинг сервера с запросом результатов изменения
		/// </summary>
		/// <param name="uid">Идентификатор клиента</param>
		/// <param name="callbackIndex">Индекс последнего обработанного сообщения</param>
		/// <returns></returns>
		[OperationContract]
		List<CallbackResult> Poll(Guid uid, int callbackIndex);

		[OperationContract]
		string Test(string arg, Guid clientUID);

		[OperationContract]
		SecurityConfiguration GetSecurityConfiguration(Guid clientUID);

		[OperationContract]
		string Ping(Guid clientUID);

		[OperationContract]
		OperationResult ResetDB(Guid clientUID);

		/// <summary>
		/// Запрос данных лицензии
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<FiresecLicenseInfo> GetLicenseInfo(Guid clientUID);
		#endregion

		#region Journal
		/// <summary>
		/// Запрос мимальной даты события, присутствующего в БД
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID);

		/// <summary>
		/// Запрос отфильтрованного списка событий
		/// </summary>
		/// <param name="journalFilter"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter journalFilter, Guid clientUID);

		[OperationContract]
		OperationResult<bool> BeginGetJournal(JournalFilter journalFilter, Guid clientUID);

		/// <summary>
		/// Добавление записи в журнал событий
		/// </summary>
		/// <param name="journalItem"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> AddJournalItem(JournalItem journalItem, Guid clientUID);

		/// <summary>
		/// Запрос списка событий на определенной странице
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUID);

		/// <summary>
		/// Запрос количества страниц событий по заданному фильтру
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<int> GetArchiveCount(JournalFilter filter, Guid clientUID);
		#endregion

		#region Files
		[OperationContract]
		List<string> GetFileNamesList(string directory, Guid clientUID);

		[OperationContract]
		Dictionary<string, string> GetDirectoryHash(string directory, Guid clientUID);

		[OperationContract]
		Stream GetServerAppDataFile(string dirAndFileName, Guid clientUID);

		[OperationContract]
		Stream GetServerFile(string filePath, Guid clientUID);

		[OperationContract]
		Stream GetConfig(Guid clientUID);

		[OperationContract]
		void SetRemoteConfig(Stream stream);

		[OperationContract]
		void SetLocalConfig(Guid clientUID);
		#endregion
	}
}