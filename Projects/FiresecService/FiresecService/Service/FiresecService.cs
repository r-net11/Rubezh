using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Timers;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Database;
using FiresecService.DatabaseConverter;
using FiresecService.Processor;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
	InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		public readonly static FiresecDbConverterDataContext DataBaseContext = ConnectionManager.CreateFiresecDataContext();
		public IFiresecCallback Callback { get; private set; }
		public CallbackWrapper CallbackWrapper { get; private set; }
		public IFiresecCallbackService FiresecCallbackService { get; private set; }
		public Guid UID { get; private set; }
		public ClientCredentials ClientCredentials { get; private set; }
		public string ClientIpAddress { get; private set; }
		public string ClientIpAddressAndPort { get; private set; }
		public bool IsSubscribed { get; private set; }
		System.Timers.Timer _recoveryTimer;
		public bool IsClientCallbackFaulted { get; private set; }

		public FiresecManager FiresecManager { get; set; }
		FiresecSerializedClient FiresecSerializedClient
		{
			get { return FiresecManager.FiresecSerializedClient; }
		}

		public FiresecService()
		{
			UID = Guid.NewGuid();
			IsClientCallbackFaulted = false;

			//_recoveryTimer = new System.Timers.Timer();
			//_recoveryTimer.Interval = 10000;
			//_recoveryTimer.Elapsed += new ElapsedEventHandler((source, e) => { ReconnectToClient(); });
		}

		public bool ReconnectToClient()
		{
			Logger.Info("FiresecService.ReconnectToClient");
			IsClientCallbackFaulted = true;
			MainViewModel.Current.UpdateClientState(UID, "Попытка соединения");
			_recoveryTimer.Stop();
			FiresecCallbackService = FiresecCallbackServiceCreator.CreateClientCallback(ClientCredentials.ClientCallbackAddress);
			try
			{
				FiresecCallbackService.Ping();
				_recoveryTimer.Stop();
				MainViewModel.Current.UpdateClientState(UID, "Норма");
			}
			catch
			{
				MainViewModel.Current.UpdateClientState(UID, "Ошибка");
				_recoveryTimer.Start();
				return false;
			}
			return false;
		}

		public OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew)
		{
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
			catch { }

			var operationResult = Authenticate(clientCredentials.UserName, clientCredentials.Password);
			if (operationResult.HasError)
				return operationResult;

			IsSubscribed = clientCredentials.ClientType != ClientType.Administrator;
			FiresecCallbackService = FiresecCallbackServiceCreator.CreateClientCallback(ClientCredentials.ClientCallbackAddress);
			Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
			CallbackWrapper = new CallbackWrapper(this);

			if (ClientsCash.IsNew(this))
			{
				DatabaseHelper.AddInfoMessage(ClientCredentials.UserName, "Вход пользователя в систему(Firesec-2)");
			}

			ClientsCash.Add(this);

			if (FiresecManager.IsConnectedToComServer)
			{
				operationResult.Result = true;
			}
			else
			{
				operationResult.HasError = false;
				operationResult.Error = "Нет соединения с ядром Firesec";
				return operationResult;
			}
			return operationResult;
		}

		public OperationResult<bool> Reconnect(string login, string password)
		{
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
			Logger.Info("FiresecService.Disconnect");
			ClientsCash.Remove(this);
			DatabaseHelper.AddInfoMessage(ClientCredentials.UserName, "Выход пользователя из системы(Firesec-2)");
		}

		public bool ContinueProgress = true;
		public void CancelProgress()
		{
			ContinueProgress = false;
			Interlocked.Exchange(ref FiresecSerializedClient.NativeFiresecClient.StopProgress, 1);
		}

		public string GetStatus()
		{
			var driversError = FiresecManager.ConfigurationConverter.DriversError.ToString();
			if (!string.IsNullOrEmpty(driversError))
			{
				return driversError;
			}
			return null;
		}

		public void ConvertConfiguration()
		{
			try
			{
				FiresecManager.Convert();
				ClientsCash.OnConfigurationChanged();
			}
			catch(Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.ConvertConfiguration");
			}
		}

		public string Ping()
		{
			return "Pong";
		}

		public string Test()
		{
			return "Test";
		}
	}
}