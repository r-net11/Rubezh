using Common;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using RubezhDAL.DataClasses;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService
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

		public OperationResult<bool> Connect(Guid clientUID, ClientCredentials clientCredentials)
		{
			if (DbService.ConnectionOperationResult.HasError && clientCredentials.ClientType != ClientType.Administrator)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			clientCredentials.ClientUID = clientUID;
			InitializeClientCredentials(clientCredentials);

			var operationResult = Authenticate(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			CurrentClientCredentials = clientCredentials;
			if (ClientsManager.Add(clientUID, clientCredentials))
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null);
			return operationResult;
		}

		public void Disconnect(Guid clientUID)
		{
			var clientInfo = ClientsManager.GetClientInfo(clientUID);
			if (clientInfo != null)
			{
				clientInfo.IsDisconnecting = true;
				clientInfo.WaitEvent.Set();
				if (clientInfo.ClientCredentials != null)
				{
					AddJournalMessage(JournalEventNameType.Выход_пользователя_из_системы, null);
				}
			}
			ClientsManager.Remove(clientUID);
		}
		public void LayoutChanged(Guid clientUID, Guid layoutUID)
		{
			var clientInfo = ClientsManager.GetClientInfo(clientUID);
			if (clientInfo != null)
				clientInfo.LayoutUID = layoutUID;
		}
		public OperationResult<ServerState> GetServerState(Guid clientUID)
		{
			return new OperationResult<ServerState>(ServerState);
		}

		public string Test(Guid clientUID, string arg)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				databaseService.PassJournalTranslator.InsertPassJournalTestData();
			}
			return "Test";
		}

		public OperationResult<SecurityConfiguration> GetSecurityConfiguration(Guid clientUID)
		{
			return ConfigurationCashHelper.GetSecurityConfiguration();
		}

		public string Ping(Guid clientUID)
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

		public OperationResult ResetDB(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ResetDB();
			}
		}

		public OperationResult<FiresecLicenseInfo> GetLicenseInfo(Guid clientUID)
		{
			return new OperationResult<FiresecLicenseInfo>(LicenseManager.CurrentLicenseInfo);
		}
	}
}