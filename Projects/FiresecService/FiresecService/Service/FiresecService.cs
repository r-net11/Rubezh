using System.Data.SqlClient;
using System.Linq;
using StrazhDeviceSDK;
using Common;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecService.Service.Validators;
using Infrastructure.Common;
using KeyGenerator;
using StrazhDAL;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using StrazhService;

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

			// Проверяем текущую конфигурацию на соответствие ограничениям лицензии
			operationResult = CheckConfigurationValidation();
			if (operationResult.HasError)
				return operationResult;

			// Проверка разрешений согласно лицензии
			operationResult = CheckConnectionRightsUsingLicenseData(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			// Проверяем количесво активных карт и сравниваем с данными лицензии
			operationResult = CheckActiveCardsCountAgainstLicenseData();
			if (operationResult.HasError)
				return operationResult;

			CurrentClientCredentials = clientCredentials;
			if (ClientsManager.Add(uid, clientCredentials))
			{
				Logger.Info(string.Format("Вход пользователя в систему: GUID='{0}' Тип='{1}' Пользователь='{2}'", clientCredentials.ClientUID, clientCredentials.ClientType, clientCredentials.FriendlyUserName));
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null);
			}

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

			Notifier.EditClient(uid, login);
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
				var clientCredentials = clientInfo.ClientCredentials;
				if (clientCredentials != null)
				{
					Logger.Info(string.Format("Выход пользователя из системы: GUID='{0}' Тип='{1}' Пользователь='{2}'", clientCredentials.ClientUID, clientCredentials.ClientType, clientCredentials.FriendlyUserName));
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

		private OperationResult<bool> CheckActiveCardsCountAgainstLicenseData()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				if (!_licenseManager.CurrentLicense.IsUnlimitedUsers &&
				    databaseService.CardTranslator.GetCardsCount() > _licenseManager.CurrentLicense.TotalUsers)
					return OperationResult<bool>.FromError("Количество активных пропусков в базе данных системы превышает лицензированное значение. Загрузка приложения невозможна");
			}
			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> CheckConfigurationValidation()
		{
			return ConfigurationElementsAgainstLicenseDataValidator.Instance.IsValidated
				? new OperationResult<bool>(true)
				: OperationResult<bool>.FromError("Конфигурация не соответствует ограничениям лицензии. Для продолжения работы загрузите другую лицензию или измените конфигурацию");
		}

		private OperationResult<bool> CheckConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			// Удаленное соединение Клиента (Администратор/ОЗ) при запрещении удаленных соединений в параметрах лицензии
			if (!NetworkHelper.IsLocalAddress(clientCredentials.ClientIpAddress) &&
				_licenseManager.CurrentLicense.OperatorConnectionsNumber == 0)
				return OperationResult<bool>.FromError("Удаленные подключения к серверу не разрешены лицензией");

			// Клиент - Администратор
			if (clientCredentials.ClientType == ClientType.Administrator)
				return CheckAdministratorConnectionRightsUsingLicenseData(clientCredentials);

			// Клиент - ОЗ
			if (clientCredentials.ClientType == ClientType.Monitor)
				return CheckMonitorConnectionRightsUsingLicenseData(clientCredentials);

			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> CheckAdministratorConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			// Может быть только одно подключение Администратора
			var existingClients = ClientsManager.ClientInfos.Where(x => x.ClientCredentials.ClientType == clientCredentials.ClientType).ToList();
			if (existingClients.Any())
				return OperationResult<bool>.FromError(string.Format(
					"Другой администратор осуществил вход с компьютера '{0}'. Одновременная работа двух администраторов в системе не допускается. Для входа в систему завершите работу на другом компьютере",
					existingClients[0].ClientCredentials.ClientIpAddress));

			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> CheckMonitorConnectionRightsUsingLicenseData(ClientCredentials clientCredentials)
		{
			var allowedConnectionsCount = _licenseManager.CurrentLicense.OperatorConnectionsNumber;

			var hasLocalMonitorConnections = ClientsManager.ClientInfos.Any(x =>
				x.ClientCredentials.ClientType == ClientType.Monitor &&
				NetworkHelper.IsLocalAddress(x.ClientCredentials.ClientIpAddress));

			var totalMonitorConnectionsCount =
				ClientsManager.ClientInfos.Count(x => x.ClientCredentials.ClientType == ClientType.Monitor);

			var isLocalClient = NetworkHelper.IsLocalAddress(clientCredentials.ClientIpAddress);
			
			if ((isLocalClient && totalMonitorConnectionsCount >= allowedConnectionsCount + 1) ||
				(!isLocalClient && hasLocalMonitorConnections && totalMonitorConnectionsCount >= allowedConnectionsCount + 1) ||
				(!isLocalClient && !hasLocalMonitorConnections && totalMonitorConnectionsCount >= allowedConnectionsCount))
				return OperationResult<bool>.FromError("Достигнуто максимальное количество подключений к серверу, допускаемое лицензией");

			// TODO: Временная мера. В рамках одного хоста может быть запущен только один экземпляр ОЗ
			var instancesRunnedFromTheSameHost = ClientsManager.ClientInfos.Where(x =>
				x.ClientCredentials.ClientType == clientCredentials.ClientType &&
				x.ClientCredentials.ClientIpAddress == clientCredentials.ClientIpAddress).ToList();
			if (instancesRunnedFromTheSameHost.Any())
				return OperationResult<bool>.FromError(string.Format(
					"Другой экземпляр ОЗ осуществил вход с компьютера '{0}'. Одновременная работа на одном ПК двух и более экземпляров ОЗ не допускается.",
					instancesRunnedFromTheSameHost[0].ClientCredentials.ClientIpAddress));

			return new OperationResult<bool>(true);
		}


		/// <summary>
		/// Получает данные лицензии с Сервера
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<LicenseData> GetLicenseData()
		{
			return new OperationResult<LicenseData>(new LicenseData
			{
				IsEnabledAutomation = _licenseManager.CurrentLicense.IsEnabledAutomation,
				IsEnabledPhotoVerification = _licenseManager.CurrentLicense.IsEnabledPhotoVerification,
				IsEnabledRVI = _licenseManager.CurrentLicense.IsEnabledRVI,
				IsEnabledURV = _licenseManager.CurrentLicense.IsEnabledURV,
				IsUnlimitedUsers = _licenseManager.CurrentLicense.IsUnlimitedUsers
			});
		}

		#endregion </Лицензирование>

		/// <summary>
		/// Получает тип оболочки рабочего стола пользователя A.C.Tech
		/// </summary>
		/// <param name="userName">Пользователь A.C.Tech</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<ShellType> GetUserShellType(string userName)
		{
			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == userName);
			{
				return new OperationResult<ShellType>(user == null ? ShellType.Default : user.ShellType);
			}
		}

		/// <summary>
		/// Получает список Клиентов Сервера
		/// </summary>
		/// <returns></returns>
		public OperationResult<List<ClientCredentials>> GetClients()
		{
			return new OperationResult<List<ClientCredentials>>(ClientsManager.ClientInfos.Select(x => x.ClientCredentials).ToList());
		}
	}
}