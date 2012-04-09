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
        public Guid FiresecServiceUID { get; private set; }
        string _userLogin;
        string _userName;
        string _userIpAddress;
        string _clientType;
        public static readonly object Locker = new object();

        public FiresecService()
        {
            FiresecServiceUID = Guid.NewGuid();
        }

        public string Connect(string clientType, string clientCallbackAddress, string login, string password)
        {
            lock (Locker)
            {
                if (!FiresecManager.IsConnected)
                    return "Нет соединения с ядром Firesec";
                //if (!string.IsNullOrEmpty(FiresecManager.DriversError))
                //    return FiresecManager.DriversError;

                if (CheckLogin(login, password))
                {
                    if (CheckRemoteAccessPermissions(login))
                    {
                        _clientType = clientType;

                        var connectionViewModel = new ConnectionViewModel()
                        {
                            FiresecServiceUID = FiresecServiceUID,
                            UserName = _userLogin,
                            IpAddress = _userIpAddress,
                            ClientType = _clientType
                        };
                        MainViewModel.Current.Connections.Add(connectionViewModel);

                        DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");

                        Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
                        CallbackManager.Add(this);

                        FiresecCallbackService = FiresecCallbackServiceCreator.CreateClientCallback(clientCallbackAddress);

                        return null;
                    }
                    return "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
                }
                return "Неверный логин или пароль";
            }
        }

        public string Reconnect(string login, string password)
        {
            var oldUserName = _userName;
            if (CheckLogin(login, password))
            {
                var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.FiresecServiceUID == FiresecServiceUID);
                connectionViewModel.UserName = login;

                //DatabaseHelper.AddInfoMessage(oldUserName, "Дежурство сдал");
                //DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял");

                return null;
            }
            return "Неверный логин или пароль";
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.FiresecServiceUID == FiresecServiceUID);
            MainViewModel.Current.RemoveConnection(connectionViewModel);

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
            //ProgressState.ContinueProgress = false;
        }

        public List<string> GetFileNamesList(string directory)
        {
            lock (Locker)
            {
                return HashHelper.GetFileNamesList(directory);
            }
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            lock (Locker)
            {
                return HashHelper.GetDirectoryHash(directory);
            }
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            lock (Locker)
            {
                var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
        }

        public void ConvertConfiguration()
        {
            ConfigurationConverter.Convert();
            CallbackManager.OnConfigurationChanged();
        }

        public void ConvertJournal()
        {
            JournalDataConverter.Convert();
        }

        public void Dispose()
        {
            Disconnect();
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
                DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");
                return "Test";
            }
        }

        bool CheckRemoteAccessPermissions(string login)
        {
            var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            _userIpAddress = endpoint.Address;

            if (CheckHostIps("localhost"))
                return true;

            var remoteAccessPermissions = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login).RemoreAccess;
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
            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
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

            var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin);
            _userName = user.Name + " (" + userIp + ")";
        }
    }
}