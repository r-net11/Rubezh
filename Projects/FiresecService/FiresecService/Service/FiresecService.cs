using System.Data.SqlClient;
using ChinaSKDDriver;
using Common;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.ViewModels;
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
			clientCredentials.ClientIpAddress = "127.0.0.1";
			clientCredentials.ClientIpAddressAndPort = "127.0.0.1:0";
			clientCredentials.UserName = clientCredentials.UserName;
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					clientCredentials.ClientIpAddress = endpoint.Address;
					clientCredentials.ClientIpAddressAndPort = endpoint.Address + ":" + endpoint.Port.ToString();
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

			var operationResult = Authenticate(clientCredentials);
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

		#region Licensing

		public OperationResult<bool> CheckLicenseExising()
		{
			return new OperationResult<bool>(_licenseManager.IsValidExistingKey());
		}

		public OperationResult<bool> CanConnect()
		{
			return new OperationResult<bool>(_licenseManager.CanConnect());
		}

		public OperationResult<bool> CanLoadModule(ModuleType type)
		{
			return new OperationResult<bool>(_licenseManager.CanLoadModule(type));
		}
		#endregion
	}
}