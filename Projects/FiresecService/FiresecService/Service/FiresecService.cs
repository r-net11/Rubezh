using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.DatabaseConverter;
using FiresecService.ViewModels;
using FiresecService.Views;
using System.ServiceModel.Description;
using Firesec;

namespace FiresecService
{
	[ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
	InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService, IDisposable
	{
		public readonly static FiresecDbConverterDataContext DataBaseContext = ConnectionManager.CreateFiresecDataContext();
		public IFiresecCallback Callback { get; private set; }
		public IFiresecCallbackService FiresecCallbackService { get; private set; }
		public Guid UID { get; private set; }
		string _userLogin;
		string _userName;
		string _userIpAddress;
		string _clientType;
		public static readonly object Locker = new object();
		FiresecSerializedClient FiresecSerializedClient;
		FiresecManager FiresecManager = new FiresecManager();
		bool IsConnectedToComServer;

		public FiresecService()
		{
			UID = Guid.NewGuid();
		}

		public OperationResult<bool> Connect(string clientType, string clientCallbackAddress, string login, string password)
		{
			lock (Locker)
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

				string oldFiresecLogin = AppSettings.OldFiresecLogin;
				string oldFiresecPassword = AppSettings.OldFiresecPassword;
				IsConnectedToComServer = FiresecManager.ConnectFiresecCOMServer(oldFiresecLogin, oldFiresecPassword);
				FiresecSerializedClient = FiresecManager.FiresecSerializedClient;

				_clientType = clientType;

				MainViewModel.Current.AddConnection(UID, _userLogin, _userIpAddress, _clientType);

				DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");

				Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
				CallbackManager.Add(this);

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

			var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.UID == UID);
			connectionViewModel.UserName = login;
			MainViewModel.Current.EditConnection(UID, login);

			DatabaseHelper.AddInfoMessage(oldUserName, "Дежурство сдал");
			DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял");

			operationResult.Result = true;
			return operationResult;
		}

		[OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
		public void Disconnect()
		{
			MainViewModel.Current.RemoveConnection(UID);
			DatabaseHelper.AddInfoMessage(_userName, "Выход пользователя из системы");
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
			if (!string.IsNullOrEmpty(FiresecManager.ConfigurationManager.DriversError))
			{
				return FiresecManager.ConfigurationManager.DriversError;
			}
			return null;
		}

		public void ConvertConfiguration()
		{
			FiresecManager.Convert();
			CallbackManager.OnConfigurationChanged();
		}

		public void ConvertJournal()
		{
			var journalDataConverter = new JournalDataConverter(FiresecSerializedClient);
			journalDataConverter.Convert();
		}

		public string Ping()
		{
			lock (Locker)
			{
				return "Pong";
			}
		}

		public string Test()
		{
			lock (Locker)
			{
				return "Test";
			}
		}

		bool CheckRemoteAccessPermissions(string login)
		{
			var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			_userIpAddress = endpoint.Address;

			if (CheckHostIps("localhost"))
				return true;

			var remoteAccessPermissions = FiresecManager.ConfigurationManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login).RemoreAccess;
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
			var user = FiresecManager.ConfigurationManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
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

			var user = FiresecManager.ConfigurationManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin);
			_userName = user.Name + " (" + userIp + ")";
		}

		public void Dispose()
		{
			Disconnect();
		}
	}
}