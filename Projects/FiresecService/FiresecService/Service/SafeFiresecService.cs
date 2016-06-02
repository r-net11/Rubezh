using Common;
using Integration.Service;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using StrazhAPI.Enums;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using KeyGenerator;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class SafeFiresecService : IFiresecService
	{
		public FiresecService FiresecService { get; set; }

		public SafeFiresecService(ILicenseManager licenseManager, IIntegrationService integrationService)
		{
			FiresecService = new FiresecService(licenseManager, integrationService);
		}

		public void BeginOperation(string operationName)
		{
		}

		public void EndOperation()
		{
		}

		private OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
				return OperationResult<T>.FromError("Ошибка при выполнении операции на сервере" + "\n\r" + e.Message + "\n" + e.StackTrace);
			}
		}

		private T SafeOperationCall<T>(Func<T> func, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
			return default(T);
		}

		private void SafeOperationCall(Action action, string operationName)
		{
			try
			{
				BeginOperation(operationName);
				action();
				EndOperation();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew)
		{
			return SafeOperationCall(() => { return FiresecService.Connect(uid, clientCredentials, isNew); }, "Connect");
		}

		public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
		}

		public void Disconnect(Guid uid)
		{
			SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public List<ServerTask> GetServerTasks()
		{
			return SafeOperationCall(() => { return FiresecService.GetServerTasks(); }, "GetServerTasks");
		}

		public OperationResult ResetDB()
		{
			return SafeOperationCall(() => { return FiresecService.ResetDB(); }, "ResetDB");
		}

		public List<CallbackResult> Poll(Guid uid)
		{
			return SafeContext.Execute<List<CallbackResult>>(() => FiresecService.Poll(uid));
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
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

		public void SetLocalConfig()
		{
			SafeOperationCall(() => { FiresecService.SetLocalConfig(); }, "SetLocalConfig");
		}

		#region Journal

		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetMinJournalDateTime());
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() => FiresecService.GetFilteredJournalItems(filter));
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.BeginGetFilteredArchive(archiveFilter, archivePortionUID));
		}

		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeOperationCall(() => { return FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
		}

		#endregion Journal

		#region Automation

		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() => { return FiresecService.RunProcedure(clientUID, procedureUID, args); }, "RunProcedure");
		}

		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeOperationCall(() => FiresecService.ProcedureCallbackResponse(procedureThreadUID, value), "ProcedureCallbackResponse");
		}

		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetProperties(layoutUID); }, "GetProperties");
		}

		#endregion Automation

		#region Reporting

		public OperationResult<List<string>>  GetAllReportNames()
		{
			return SafeOperationCall(() => FiresecService.GetAllReportNames(), "GetAllReportNames");
		}
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
		public OperationResult<bool> CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName,
			bool useIntegratedSecurity, string userID, string userPwd)
		{
			return SafeOperationCall(() =>
			{
				return FiresecService.CheckSqlServerConnection(ipAddress, ipPort, instanceName, useIntegratedSecurity, userID, userPwd);
			}, "CheckSqlServerConnection");
		}

		#region Licensing

		public OperationResult<bool> CheckLicenseExising()
		{
			return SafeOperationCall(() => FiresecService.CheckLicenseExising(), "CheckLicenseExising");
		}

		public OperationResult<bool> CanConnect()
		{
			return SafeOperationCall(() => FiresecService.CanConnect(), "CanConnect");
		}

		public OperationResult<bool> CanLoadModule(ModuleType type)
		{
			return SafeOperationCall(() => FiresecService.CanLoadModule(type), "CanLoadModule");
		}

		/// <summary>
		/// Получает данные лицензии с Сервера
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<LicenseData> GetLicenseData()
		{
			return SafeOperationCall(() => FiresecService.GetLicenseData(), "GetLicenseData");
		}
	
		#endregion

		/// <summary>
		/// Посылает команду Клиенту на закрытие соединения с Сервером
		/// </summary>
		/// <param name="clientUid">Идентификатор клиента, которому посылается команда</param>
		public void SendDisconnectClientCommand(Guid clientUid)
		{
			SafeOperationCall(() => FiresecService.SendDisconnectClientCommand(clientUid), "SendDisconnectClientCommand");
		}

		/// <summary>
		/// Монитор Сервера уведомляет Сервер о смене лицензии
		/// </summary>
		public void NotifyLicenseChanged()
		{
			SafeOperationCall(() => FiresecService.NotifyLicenseChanged(), "NotifyLicenseChanged");
		}

		/// <summary>
		/// Получает тип оболочки рабочего стола пользователя A.C.Tech
		/// </summary>
		/// <param name="userName">Пользователь A.C.Tech</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<ShellType> GetUserShellType(string userName)
		{
			return SafeOperationCall(() => FiresecService.GetUserShellType(userName), "GetUserShellType");
		}

		/// <summary>
		/// Получает список Клиентов Сервера
		/// </summary>
		/// <returns></returns>
		public OperationResult<List<ClientCredentials>> GetClients()
		{
			return SafeOperationCall(() => FiresecService.GetClients(), "GetClients");
		}

		/// <summary>
		/// Получает логи загрузки Сервера
		/// </summary>
		/// <returns>Логи загрузки Сервера</returns>
		public OperationResult<string> GetLogs()
		{
			return SafeOperationCall(() => FiresecService.GetLogs(), "GetLogs");
		}

		/// <summary>
		/// Уведомление об изменении лога загрузки Сервера
		/// </summary>
		public void NotifyCoreLoadingLogChanged()
		{
			SafeOperationCall(() => FiresecService.NotifyCoreLoadingLogChanged(), "NotifyCoreLoadingLogChanged");
		}
	}
}