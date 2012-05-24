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
		string _userLogin;
		string _userName;
		string _userIpAddress;
		string _clientType;
		public static readonly object Locker = new object();

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

		public void BeginOperation(string operationName)
		{
			MainViewModel.Current.UpdateCurrentOperationName(UID, operationName);
		}

		public void EndOperation()
		{
			MainViewModel.Current.UpdateCurrentOperationName(UID, "");
		}

		public void DisposeComServer()
		{
			Logger.Info("DisposeComServer");
			ServiceCash.Free(FiresecManager);
		}

		public OperationResult<bool> Connect(string clientType, string clientCallbackAddress, string login, string password)
		{
			FiresecManager.LoadConfiguration();

			var operationResult = new OperationResult<bool>();

			if (CheckLogin(login, password) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "Неверный логин или пароль";
				return operationResult;
			}
			if (CheckRemoteAccessPermissions(login) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
				return operationResult;
			}

			_clientType = clientType;

			MainViewModel.Current.AddConnection(this, UID, _userLogin, _userIpAddress, _clientType);

			Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
			CallbackWrapper = new CallbackWrapper(this);
			CallbackManager.Add(this);

			//FiresecManager.ConnectFiresecCOMServer();

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
			var operationResult = new OperationResult<bool>();
			var oldUserName = _userName;

			if (CheckLogin(login, password) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "Неверный логин или пароль";
				return operationResult;
			}
			if (CheckRemoteAccessPermissions(login) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
				return operationResult;
			}

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

		bool CheckRemoteAccessPermissions(string login)
		{
			var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			_userIpAddress = endpoint.Address;

			if (CheckHostIps("localhost"))
				return true;

			var remoteAccessPermissions = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login).RemoreAccess;
			if (remoteAccessPermissions == null)
				return false;

			switch (remoteAccessPermissions.RemoteAccessType)
			{
				case RemoteAccessType.RemoteAccessBanned:
					return false;

				case RemoteAccessType.RemoteAccessAllowed:
					return true;

				case RemoteAccessType.SelectivelyAllowed:
					foreach (var hostNameOrIpAddress in remoteAccessPermissions.HostNameOrAddressList)
					{
						if (CheckHostIps(hostNameOrIpAddress))
							return true;
					}
					break;
			}
			return false;
		}

		bool CheckHostIps(string hostNameOrIpAddress)
		{
			try
			{
				var addressList = Dns.GetHostEntry(hostNameOrIpAddress).AddressList;
				return addressList.Any(ip => ip.ToString() == _userIpAddress);
			}
			catch
			{
				return false;
			}
		}

		bool CheckLogin(string login, string password)
		{
			var user = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
			if (user == null || !HashHelper.CheckPass(password, user.PasswordHash))
				return false;

			_userLogin = login;
			SetUserFullName();

			return true;
		}

		void SetUserFullName()
		{
			var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			string userIp = endpoint.Address;

			var addressList = Dns.GetHostEntry("localhost").AddressList;
			if (addressList.Any(ip => ip.ToString() == userIp))
				userIp = "localhost";

			var user = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin);
			_userName = user.Name + " (" + userIp + ")";
		}

		public void Dispose()
		{
			Disconnect();
		}
	}
}