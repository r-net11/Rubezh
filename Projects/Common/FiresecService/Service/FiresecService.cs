using Common;
using Infrastructure.Common.License;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using RubezhDAL.DataClasses;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService
	{
		public static readonly Guid UID = Guid.NewGuid();
		public static ServerState ServerState { get; set; }

		public static event Action<Guid> AfterConnect;

		static string GetUserName(Guid? clientUID, string userName = null)
		{
			if (userName == null)
			{
				var clientInfo = clientUID.HasValue ? ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID.Value) : null;
				return clientInfo == null ? "<Нет>" : clientInfo.ClientCredentials.FriendlyUserName;
			}
			else
				return userName;
		}

		static string GetLogin(Guid clientUID)
		{
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
			return clientInfo == null ? null : clientInfo.ClientCredentials.Login;
		}

		void InitializeClientCredentials(ClientCredentials clientCredentials)
		{
			clientCredentials.ClientIpAddress = "127.0.0.1";
			clientCredentials.ClientIpAddressAndPort = "127.0.0.1:0";
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

		public OperationResult<bool> Connect(ClientCredentials clientCredentials)
		{
			if (DbService.ConnectionOperationResult.HasError && clientCredentials.ClientType != ClientType.Administrator)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			InitializeClientCredentials(clientCredentials);

			var operationResult = Authenticate(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			if (ClientsManager.Add(clientCredentials))
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null, null, clientCredentials.ClientUID);
			if (AfterConnect != null)
				AfterConnect(clientCredentials.ClientUID);
			return operationResult;
		}

		public void Disconnect(Guid clientUID)
		{
			var clientInfo = ClientsManager.GetClientInfo(clientUID);
			if (clientInfo != null)
			{
				clientInfo.WaitEvent.Set();
				if (clientInfo.ClientCredentials != null)
				{
					AddJournalMessage(JournalEventNameType.Выход_пользователя_из_системы, null, null, clientUID);
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

		public OperationResult<bool> ResetDB(Guid clientUID)
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