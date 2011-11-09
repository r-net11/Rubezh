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
using FiresecServiceRunner;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
        InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class FiresecService : IFiresecService, IDisposable
    {
        public readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();
        public IFiresecCallback Callback { get; private set; }
        string _userLogin;
        string _userName;
        string _userIpAddress;
        public static readonly object Locker = new object();

        public string Connect(string login, string password)
        {
            lock (Locker)
            {
                if (CheckLogin(login, password))
                {
                    if (CheckRemoteAccessPermissions(login))
                    {
                        MainWindow.AddMessage("Connected. UserName = " + login + ". SessionId = " + OperationContext.Current.SessionId);
                        DatabaseHelper.AddInfoMessage(_userName, "Вход пользователя в систему");

                        Callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
                        CallbackManager.Add(this);

                        return null;
                    }
                    return "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
                }
                return "Неверный логин или пароль";
            }
        }

        public string Reconnect(string login, string password)
        {
            var oldUserFileName = _userName;
            if (CheckLogin(login, password))
            {
                DatabaseHelper.AddInfoMessage(oldUserFileName, "Дежурство сдал");
                DatabaseHelper.AddInfoMessage(_userName, "Дежурство принял");

                return null;
            }
            return "Неверный логин или пароль";
        }

        [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.AfterCall)]
        public void Disconnect()
        {
            DatabaseHelper.AddInfoMessage(_userName, "Выход пользователя из системы");
            CallbackManager.Remove(this);
        }

        public bool IsSubscribed { get; set; }
        public void Subscribe()
        {
            IsSubscribed = true;
        }

        public bool MustStopProgress;
        public void StopProgress()
        {
            MustStopProgress = true;
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
            return true;
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