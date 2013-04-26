using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Database;
using FiresecService.DatabaseConverter;
using FiresecService.Properties;
using FiresecService.ViewModels;
using FiresecService.Processor;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		public static readonly SqlCeConnection DataBaseContext =
			new SqlCeConnection(Settings.Default.FiresecConnectionString);

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
			ConfigurationCash.SecurityConfiguration = SecurityConfigurationHelper.GetSecurityConfiguration();

			clientCredentials.ClientUID = uid;
			InitializeClientCredentials(clientCredentials);

			var operationResult = Authenticate(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			if (ClientsManager.Add(uid, clientCredentials))
			{
				AddInfoMessage(clientCredentials.FriendlyUserName, "Вход пользователя в систему(Firesec-2)");
			}

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
			AddInfoMessage(oldUserName, "Дежурство сдал(Firesec-2)");
			clientCredentials.UserName = login;
			SetUserFullName(clientCredentials);
			AddInfoMessage(clientCredentials.FriendlyUserName, "Дежурство принял(Firesec-2)");

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
					AddInfoMessage(clientInfo.ClientCredentials.FriendlyUserName, "Выход пользователя из системы(Firesec-2)");
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
			return SecurityConfigurationHelper.GetSecurityConfiguration();
		}
	}
}