using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhAPI.License;
using RubezhAPI.Automation;
using OpcClientSdk;
using OpcClientSdk.Da;

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
		/// <param name="isNew">Признак того, что это новое подключение, а не переподключение</param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew);

		/// <summary>
		/// Отсоединение от сервиса
		/// </summary>
		/// <param name="uid">Идентификатор клиента</param>
		[OperationContract(IsOneWay = true)]
		void Disconnect(Guid uid);

		[OperationContract]
		OperationResult<ServerState> GetServerState();

		/// <summary>
		/// Поллинг сервера с запросом результатов изменения
		/// Метод реализует концепцию лонг-пол. Т.е. возвращает результат сразу, если есть изменения. Если изменений нет, то он блокируется либо до истечения таймаута в несколько минут, либо до изменения состояния объектов или появлении нового события. Поллинг сервера с запросом результатов изменения
		/// </summary>
		/// <param name="uid">Идентификатор клиента</param>
		/// <returns></returns>
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

		/// <summary>
		/// Запрос данных лицензии
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<FiresecLicenseInfo> GetLicenseInfo();
		#endregion

		#region Journal
		/// <summary>
		/// Запрос мимальной даты события, присутствующего в БД
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<DateTime> GetMinJournalDateTime();

		/// <summary>
		/// Запрос отфильтрованного списка событий
		/// </summary>
		/// <param name="journalFilter"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter journalFilter);

		[OperationContract]
		OperationResult<bool> BeginGetJournal(JournalFilter journalFilter);

		/// <summary>
		/// Добавление записи в журнал событий
		/// </summary>
		/// <param name="journalItem"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> AddJournalItem(JournalItem journalItem);

		/// <summary>
		/// Запрос списка событий на определенной странице
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page);

		/// <summary>
		/// Запрос количества страниц событий по заданному фильтру
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<int> GetArchiveCount(JournalFilter filter);
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

		#region OPC DA

		/// <summary>
		/// Возвращает список имен OPC DA серверов, 
		/// зарегистрированных в системе
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcDaServer[]> GetOpcDaServers();

		/// <summary>
		/// Возвращает структуру сервера (группы и теги) для указанного сервера
		/// </summary>
		/// <param name="server">Наименование сервера</param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcDaServer> GetOpcDaServerGroupAndTags(OpcDaServer server);

		/// <summary>
		/// Подключается к указанному OPC DA серверу и
		/// подписывается на получение значений тегов указанных в модели
		/// </summary>
		/// <param name="server">OPC DA сервер для подключения</param>
		/// <returns></returns>
		[OperationContract]
		OperationResult ConnectToOpcDaServer(OpcDaServer server);

		/// <summary>
		/// Отключается от указанного OPC DA сервера и
		/// прекращает подписку на получение значениий тегов
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult DisconnectFromOpcDaServer(OpcDaServer server);

		/// <summary>
		/// Возвращает описание и текущий статус сервера
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<OpcServerStatus> GetOpcDaServerStatus(OpcDaServer server);

		/// <summary>
		/// Возвращает значения тегов
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult<TsCDaItemValueResult[]> ReadOpcDaServerTags(OpcDaServer server);

		/// <summary>
		/// Записывает новое значение указанных тегов
		/// </summary>
		/// <param name="server"></param>
		/// <param name="tagValues"></param>
		/// <returns></returns>
		[OperationContract]
		OperationResult WriteOpcDaServerTags(OpcDaServer server, TsCDaItemValue[] tagValues);
		#endregion
	}
}