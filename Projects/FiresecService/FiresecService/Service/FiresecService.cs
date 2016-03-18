using System.Data.SqlClient;
using System.Linq;
using ChinaSKDDriver;
using Common;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.ViewModels;
using Infrastructure.Common;
using KeyGenerator;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		private readonly ILicenseManager _licenseManager;

		public FiresecService(ILicenseManager licenseManager)
		{
			if(licenseManager == null)
				throw new ArgumentException("License Manager");

			_licenseManager = licenseManager;

			// Записываем состояние лицензии в журнал системы при запуске сервера
			AddJournalMessage(_licenseManager.IsValidExistingKey() ? JournalEventNameType.Лицензия_обнаружена : JournalEventNameType.Отсутствует_лицензия, null);
			
		}

		private ClientCredentials CurrentClientCredentials;

		private void InitializeClientCredentials(ClientCredentials clientCredentials)
		{
			clientCredentials.ClientIpAddress = NetworkHelper.LocalhostIp; // 127.0.0.1
			clientCredentials.ClientIpAddressAndPort = string.Format("{0}:0", NetworkHelper.LocalhostIp); // "127.0.0.1:0";
			clientCredentials.UserName = clientCredentials.UserName;
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					clientCredentials.ClientIpAddress = endpoint.Address;
					clientCredentials.ClientIpAddressAndPort = string.Format("{0}:{1}", endpoint.Address, endpoint.Port);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.InitializeClientCredentials");
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew)
		{
			clientCredentials.ClientUID = uid;
			InitializeClientCredentials(clientCredentials);

			// Проводим аутентификацию пользователя
			var operationResult = Authenticate(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			// Проверяем активацию лицензии на сервере
			operationResult = CanConnect();
			if (operationResult.HasError)
				return operationResult;

			// Проверка разрешений согласно лицензии
			operationResult = CheckConnectionRightsUsingLicenseData(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			CurrentClientCredentials = clientCredentials;
			if (ClientsManager.Add(uid, clientCredentials))
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null);

			return operationResult;
		}

		public OperationResult<bool> Reconnect(Guid uid, string login, string password)
		{
			var clientCredentials = ClientsManager.GetClientCredentials(uid);
			if (clientCredentials == null)
			{
				return OperationResult<bool>.FromError("Не найден пользователь");
			}
			InitializeClientCredentials(clientCredentials);
			var oldUserName = clientCredentials.FriendlyUserName;

			var newClientCredentials = new ClientCredentials()
			{
				UserName = login,
				Password = password,
				ClientIpAddress = clientCredentials.ClientIpAddress
			};
			var operationResult = Authenticate(newClientCredentials);
			if (operationResult.HasError)
				return operationResult;

			MainViewModel.Current.EditClient(uid, login);
			AddJournalMessage(JournalEventNameType.Дежурство_сдал, null, JournalEventDescriptionType.NULL, oldUserName);
			clientCredentials.UserName = login;
			SetUserFullName(clientCredentials);
			AddJournalMessage(JournalEventNameType.Дежурство_принял, null, JournalEventDescriptionType.NULL, clientCredentials.FriendlyUserName);

			CurrentClientCredentials = clientCredentials;
			operationResult.Result = true;
			return operationResult;
		}

		public void Disconnect(Guid uid)
		{
			var clientInfo = ClientsManager.GetClientInfo(uid);
			if (clientInfo != null)
			{
				if (clientInfo.ClientCredentials.ClientType == ClientType.Monitor)
				{
					if (FiresecService.CurrentThread != null)
					{
						DBHelper.IsAbort = true;
						CurrentThread.Join(TimeSpan.FromSeconds(2));
						CurrentThread = null;
					}
				}
				clientInfo.IsDisconnecting = true;
				clientInfo.WaitEvent.Set();
				if (clientInfo.ClientCredentials != null)
				{
					AddJournalMessage(JournalEventNameType.Выход_пользователя_из_системы, null);
				}
			}
			ClientsManager.Remove(uid);
		}

		public void NotifyClientsOnConfigurationChanged()
		{
			NotifyConfigurationChanged();
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return ConfigurationCashHelper.GetSecurityConfiguration();
		}

		public string Ping()
		{
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					return endpoint.Address;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.Ping");
			}
			return null;
		}

		public List<ServerTask> GetServerTasks()
		{
			try
			{
				var result = new List<ServerTask>();
				using (var databaseService = new SKDDatabaseService())
				{
					foreach (var device in SKDManager.Devices)
					{
						if (device.Driver.IsController)
						{
							var pendingCards = databaseService.CardTranslator.GetAllPendingCards(device.UID);
							foreach (var pendingCard in pendingCards)
							{
								var serverTask = new ServerTask();
								serverTask.DeviceName = device.Name;
								serverTask.DeviceAddress = device.Address;
								if (pendingCard.Card != null)
									serverTask.CardNumber = pendingCard.Card.Number;
								serverTask.PendingCardAction = (PendingCardAction)pendingCard.Action;
								result.Add(serverTask);
							}
						}
					}
				}
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetServerTasks");
			}
			return null;
		}

		public OperationResult ResetDB()
		{
			return PatchManager.ResetDB();
		}

		public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			Processor.CancelProgress(progressCallbackUID, userName);
		}

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
		public OperationResult<bool> CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName, bool useIntegratedSecurity, string userID, string userPwd)
		{
			var connectionString = SKDDatabaseService.BuildConnectionString(ipAddress, ipPort, instanceName, "master", useIntegratedSecurity, userID, userPwd);
			using (var connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch (Exception e)
				{
					return OperationResult<bool>.FromError(e.Message);
				}
			}
			return new OperationResult<bool>(true);
		}

		#region <Лицензирование>

		public OperationResult<bool> CheckLicenseExising()
		{
			return new OperationResult<bool>(_licenseManager.IsValidExistingKey());
		}

		public OperationResult<bool> CanConnect()
		{
			return _licenseManager.CanConnect()
				? new OperationResult<bool>(true)
				: OperationResult<bool>.FromError("Сервер не активирован. Подключение к серверу возможно только после его активации");
		}

		public OperationResult<bool> CanLoadModule(ModuleType type)
		{
			return new OperationResult<bool>(_licenseManager.CanLoadModule(type));
		}

		private OperationResult<bool> CheckConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			// Если клиент запущен локально, то закрыть прочие локальные клиенты
			if (NetworkHelper.IsLocalAddress(clientCredentials.ClientIpAddress))
			{
				var existingClients = ClientsManager.ClientInfos.Where(x =>
					x.ClientCredentials.ClientIpAddress == clientCredentials.ClientIpAddress &&
					x.ClientCredentials.ClientType == clientCredentials.ClientType);
				foreach (var existingClient in existingClients)
				{
					SendCloseClientCommand(existingClient.ClientCredentials.ClientUID);
				}

				return new OperationResult<bool>(true);
			}
			
			// Остальные проверки выполняются только для удаленного клиента
			if (_licenseManager.CurrentLicense.OperatorConnectionsNumber == 0)
				return OperationResult<bool>.FromError("Удаленные подключения к серверу не разрешены лицензией");

			if (clientCredentials.ClientType == ClientType.Administrator)
				return CheckAdministratorConnectionRightsUsingLicenseData(clientCredentials);

			if (clientCredentials.ClientType == ClientType.Monitor)
				return CheckMonitorConnectionRightsUsingLicenseData(clientCredentials);

			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> CheckAdministratorConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			// Значение опции "Оперативная задача (подключение)"
			var allowedRemoteCoonnectionsNumber = _licenseManager.CurrentLicense.OperatorConnectionsNumber;
			
			// В списке подключений есть подключение "Администратора", установленное с другого компьютера
			var clientsFromOtherHosts = ClientsManager.ClientInfos.Where(x =>
				x.ClientCredentials.ClientIpAddress != clientCredentials.ClientIpAddress &&
				x.ClientCredentials.ClientType == clientCredentials.ClientType).ToList();
			var hasClientsFromOtherHosts = clientsFromOtherHosts.Any();

			// В списке подключений есть подключение "Администратора", установленное с данного компьютера
			var clientsFromThisHost = ClientsManager.ClientInfos.Where(x =>
				x.ClientCredentials.ClientIpAddress == clientCredentials.ClientIpAddress &&
				x.ClientCredentials.ClientType == clientCredentials.ClientType).ToList();
			var hasClientsFromThisHost = clientsFromThisHost.Any();

			if (allowedRemoteCoonnectionsNumber > 0 && !hasClientsFromOtherHosts && !hasClientsFromThisHost)
				return new OperationResult<bool>(true);

			if (allowedRemoteCoonnectionsNumber > 0 && hasClientsFromOtherHosts && !hasClientsFromThisHost)
				return OperationResult<bool>.FromError(string.Format(
					"Другой администратор осуществил вход с компьютера '{0}'. Одновременная работа двух администраторов в системе не допускается. Для входа в систему завершите работу на другом компьютере",
					clientsFromOtherHosts[0].ClientCredentials.ClientIpAddress));

			if (allowedRemoteCoonnectionsNumber > 0 && !hasClientsFromOtherHosts && hasClientsFromThisHost)
			{
				foreach (var clientFromThisHost in clientsFromThisHost)
				{
					SendCloseClientCommand(clientFromThisHost.ClientCredentials.ClientUID);
				}
				return new OperationResult<bool>(true);
			}

			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> CheckMonitorConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			return new OperationResult<bool>(true);
		}

		#endregion </Лицензирование>
	}
}