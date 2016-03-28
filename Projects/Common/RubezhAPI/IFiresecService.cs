using OpcClientSdk;
using RubezhAPI.Automation;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Windows.Media;

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
		OperationResult<bool> Connect(ClientCredentials clientCredentials);

		/// <summary>
		/// Отсоединение от сервиса
		/// </summary>
		/// <param name="uid">Идентификатор клиента</param>
		[OperationContract]
		void Disconnect(Guid clientUID);

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
		[ServiceKnownType(typeof(Color))]
		PollResult Poll(Guid clientUID, int callbackIndex);

		[OperationContract]
		void LayoutChanged(Guid clientUID, Guid layoutUID);

		[OperationContract]
		string Test(Guid clientUID, string arg);

		[OperationContract]
		OperationResult<SecurityConfiguration> GetSecurityConfiguration(Guid clientUID);

		[OperationContract]
		void SetSecurityConfiguration(Guid clientUID, SecurityConfiguration securityConfiguration);

		[OperationContract]
		string Ping(Guid clientUID);

		[OperationContract]
		OperationResult<bool> ResetDB(Guid clientUID);

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
		OperationResult<List<JournalItem>> GetFilteredJournalItems(Guid clientUID, JournalFilter journalFilter);

		[OperationContract]
		OperationResult<bool> BeginGetJournal(JournalFilter journalFilter, Guid clentUid, Guid journalClientUid);

		/// <summary>
		/// Добавление записи в журнал событий
		/// </summary>
		/// <param name="journalItem"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> AddJournalItem(Guid clientUID, JournalItem journalItem);

		/// <summary>
		/// Запрос списка событий на определенной странице
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clentUid, string userName);

		/// <summary>
		/// Запрос количества страниц событий по заданному фильтру
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<int> GetArchiveCount(Guid clientUID, JournalFilter filter);
		#endregion

		#region Files
		[OperationContract]
		List<string> GetFileNamesList(Guid clientUID, string directory);

		[OperationContract]
		Dictionary<string, string> GetDirectoryHash(Guid clientUID, string directory);

		[OperationContract]
		Stream GetServerAppDataFile(Guid clientUID, string dirAndFileName);

		[OperationContract]
		Stream GetServerFile(Guid clientUID, string filePath);

		[OperationContract]
		Stream GetConfig(Guid clientUID);

		[OperationContract]
		void SetRemoteConfig(Stream stream);

		[OperationContract]
		void SetLocalConfig(Guid clientUID);
		#endregion

		#region OPC DA

		/// <summary>
		/// Возвращает список имен OPC DA серверов, 
		/// зарегистрированных в системе
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcDaServer[]> GetOpcDaServers(Guid clientUID);

		/// <summary>
		/// Возвращает структуру сервера (группы и теги) для указанного сервера
		/// </summary>
		/// <param name="clientUID"></param>
		/// <param name="server">Наименование сервера</param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, OpcDaServer server);

		/// <summary>
		/// Возвращает описание и текущий статус сервера
		/// </summary>
		/// <param name="clientUID"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, OpcDaServer server);

		/// <summary>
		/// Возвращает значения тегов
		/// </summary>
		/// <param name="clientUID"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(Guid clientUID, OpcDaServer server);

		/// <summary>
		/// Записывает новое значение указанных тегов
		/// </summary>
		/// <param name="clientUID"></param>
		/// <param name="serverId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> WriteOpcDaTag(Guid clientUID, Guid serverId, object value);

		#endregion
	}
}