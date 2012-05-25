using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.ViewModels;
using FiresecService.Database;
using FiresecService.Processor;
using FiresecService.DatabaseConverter;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
	InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService, IDisposable
	{
		public readonly static FiresecDbConverterDataContext DataBaseContext = ConnectionManager.CreateFiresecDataContext();
		public IFiresecCallback Callback { get; private set; }
		public CallbackWrapper CallbackWrapper { get; private set; }
		public IFiresecCallbackService FiresecCallbackService { get; private set; }
		public Guid UID { get; private set; }
		public Guid ClientUID { get; private set; }
		string _userLogin;
		string _userName;
		string _userIpAddress;
		string _clientType;

		public FiresecManager FiresecManager { get; private set; }
		FiresecSerializedClient FiresecSerializedClient
		{
			get { return FiresecManager.FiresecSerializedClient; }
		}
		bool IsConnectedToComServer = true;

		public FiresecService()
		{
			UID = Guid.NewGuid();
			FiresecManager = ServiceCash.Get(this);
		}

		public void Free()
		{
			Logger.Info("Free FiresecService");
			ServiceCash.Free(FiresecManager);
		}

		public OperationResult<bool> Connect(Guid clientUID, string clientType, string clientCallbackAddress, string login, string password)
		{
			var operationResult = Authenticate(login, password);
			if (operationResult.HasError)
				return operationResult;

			_clientType = clientType;
			ClientUID = clientUID;

			MainViewModel.Current.AddConnection(this, UID, _userLogin, _userIpAddress, _clientType);

			Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
			CallbackWrapper = new CallbackWrapper(this);
			CallbackManager.Add(this);

			DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему(Firesec-2)");

			FiresecCallbackService = FiresecCallbackServiceCreator.CreateClientCallback(clientCallbackAddress);

			if (IsConnectedToComServer)
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
			var oldUserName = _userName;

			var operationResult = Authenticate(login, password);
			if (operationResult.HasError)
				return operationResult;

			MainViewModel.Current.EditConnection(UID, login);

			DatabaseHelper.AddInfoMessage(oldUserName, "Дежурство сдал(Firesec-2)");
			DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял(Firesec-2)");

			operationResult.Result = true;
			return operationResult;
		}

		[OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
		public void Disconnect()
		{
			ServiceCash.Free(FiresecManager);
			MainViewModel.Current.RemoveConnection(UID);
			DatabaseHelper.AddInfoMessage(_userName, "Выход пользователя из системы(Firesec-2)");
			CallbackManager.Remove(this);
		}

		public bool IsSubscribed { get; private set; }
		public void Subscribe()
		{
			IsSubscribed = true;
		}

		public bool ContinueProgress = true;
		public void CancelProgress()
		{
			ContinueProgress = false;
		}

		public string GetStatus()
		{
			if (!string.IsNullOrEmpty(FiresecManager.ConfigurationConverter.DriversError))
			{
				return FiresecManager.ConfigurationConverter.DriversError;
			}
			return null;
		}

		public void ConvertConfiguration()
		{
			FiresecManager.Convert();
			CallbackManager.OnConfigurationChanged();
		}

		public string Ping()
		{
			return "Pong";
		}

		public string Test()
		{
			return "Test";
		}

		public void BeginOperation(string operationName)
		{
			MainViewModel.Current.UpdateCurrentOperationName(UID, operationName);
		}

		public void EndOperation()
		{
			MainViewModel.Current.UpdateCurrentOperationName(UID, "");
		}

		public void Dispose()
		{
			//Disconnect();
		}
	}
}