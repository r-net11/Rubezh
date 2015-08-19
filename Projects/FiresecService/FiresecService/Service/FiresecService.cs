using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.ViewModels;
using SKDDriver;
using SKDDriver.DataClasses;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		ClientCredentials CurrentClientCredentials;
		public static ServerState ServerState { get; set; }

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

			CurrentClientCredentials = clientCredentials;
			if (ClientsManager.Add(uid, clientCredentials))
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null);
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
						DbService.IsAbort = true;
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

		public OperationResult<ServerState> GetServerState()
		{
			return new OperationResult<ServerState>(ServerState);
		}

		public string Test(string arg)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				dbService.PassJournalTranslator.InsertPassJournalTestData();
			}
			return "Test";
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

		public OperationResult ResetDB()
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.ResetDB();
			}
		}
	}
}