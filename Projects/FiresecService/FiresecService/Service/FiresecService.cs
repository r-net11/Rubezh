using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Database;
using FiresecService.DatabaseConverter;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
    [ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class FiresecService : IFiresecService
    {
        public readonly static FiresecDbConverterDataContext DataBaseContext = ConnectionManager.CreateFiresecDataContext();
        public Guid UID { get; private set; }
        public ClientCredentials ClientCredentials { get; private set; }
        public string ClientIpAddress { get; private set; }
        public string ClientIpAddressAndPort { get; private set; }

        public FiresecService()
        {
            UID = Guid.NewGuid();
        }

		public OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew)
        {
            CallbackResults = new List<CallbackResult>();

            ClientCredentials = clientCredentials;
            ClientIpAddress = "127.0.0.1";
            ClientIpAddressAndPort = "127.0.0.1:0";
            try
            {
                if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
                {
                    var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    ClientIpAddress = endpoint.Address;
                    ClientIpAddressAndPort = endpoint.Address + ":" + endpoint.Port.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecService.Connect");
            }

            var operationResult = Authenticate(clientCredentials.UserName, clientCredentials.Password);
            if (operationResult.HasError)
                return operationResult;

            if (ClientsCash.IsNew(this))
            {
            }

            ClientsCash.Add(this);
            DatabaseHelper.AddInfoMessage(ClientCredentials.UserName, "Вход пользователя в систему(Firesec-2)");

            return operationResult;
        }

        public OperationResult<bool> Reconnect(string login, string password)
        {
            CallbackResults = new List<CallbackResult>();

            var oldUserName = ClientCredentials.UserName;

            var operationResult = Authenticate(login, password);
            if (operationResult.HasError)
                return operationResult;

            MainViewModel.Current.EditClient(UID, login);

            DatabaseHelper.AddInfoMessage(oldUserName, "Дежурство сдал(Firesec-2)");
            DatabaseHelper.AddInfoMessage(ClientCredentials.UserName, "Дежурство принял(Firesec-2)");

            ClientCredentials.UserName = login;

            operationResult.Result = true;
            return operationResult;
        }

        public void Disconnect()
        {
            DatabaseHelper.AddInfoMessage(ClientCredentials.UserName, "Выход пользователя из системы(Firesec-2)");
            ClientsCash.Remove(this);
        }

        public string GetStatus()
        {
            return null;
        }

        public string Ping()
        {
            return "Pong";
        }

        public void NotifyClientsOnConfigurationChanged()
        {
			ClientsCash.OnConfigurationChanged();
        }
	}
}