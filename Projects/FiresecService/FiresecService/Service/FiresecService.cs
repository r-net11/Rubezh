using System;
using System.Data.SqlServerCe;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecService.Properties;
using FiresecService.ViewModels;
using GKProcessor;
using SKDDriver;
using System.Collections.Generic;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		public static readonly SqlCeConnection DataBaseContext = new SqlCeConnection(Settings.Default.FiresecConnectionString);
		ClientCredentials CurrentClientCredentials;

		void InitializeClientCredentials(ClientCredentials clientCredentials)
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

			if (ClientsManager.Add(uid, clientCredentials))
			{
				GKProcessorManager.AddGKMessage(JournalEventNameType.Вход_пользователя_в_систему, JournalEventDescriptionType.NULL, "", null, clientCredentials.FriendlyUserName);
			}

			CurrentClientCredentials = clientCredentials;
			return operationResult;
		}

		public OperationResult<bool> Reconnect(Guid uid, string login, string password)
		{
			var clientCredentials = ClientsManager.GetClientCredentials(uid);
			if (clientCredentials == null)
			{
				return new OperationResult<bool>("Не найден пользователь");
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
			GKProcessorManager.AddGKMessage(JournalEventNameType.Дежурство_сдал, JournalEventDescriptionType.NULL, "", null, oldUserName);
			clientCredentials.UserName = login;
			SetUserFullName(clientCredentials);
			GKProcessorManager.AddGKMessage(JournalEventNameType.Дежурство_принял, JournalEventDescriptionType.NULL, "", null, clientCredentials.FriendlyUserName);

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
						FiresecDB.DatabaseHelper.IsAbort = true;
						CurrentThread.Join();
						CurrentThread = null;
					}
				}
				clientInfo.IsDisconnecting = true;
				clientInfo.WaitEvent.Set();
				if (clientInfo.ClientCredentials != null)
				{
					GKProcessorManager.AddGKMessage(JournalEventNameType.Выход_пользователя_из_системы, JournalEventDescriptionType.NULL, "", null, clientInfo.ClientCredentials.FriendlyUserName);
				}
			}
			ClientsManager.Remove(uid);
		}

		public string Test(string arg)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				databaseService.TimeTrackTranslator.InsertPassJournalTestData();
			}
			return "Test";
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
				for (int i = 0; i < 10; i++)
				{
					var serverTask = new ServerTask();
					serverTask.Name = "Name_" + i.ToString();
					result.Add(serverTask);
				}
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetServerTasks");
			}
			return null;
		}
	}
}