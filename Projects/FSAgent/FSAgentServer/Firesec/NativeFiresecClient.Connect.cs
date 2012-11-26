using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using Infrastructure.Common;

namespace FSAgentServer
{
    public partial class NativeFiresecClient
    {
        int _reguestId = 1;

        public NativeFiresecClient()
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
            {
                if (Connection != null)
                {
                    StopConnectionTimeoutThread();
                    Marshal.FinalReleaseComObject(Connection);
                    Connection = null;
                    GC.Collect();
                }
            };
        }

        FS_Types.IFSC_Connection Connection;
        bool IsConnected = false;
        string Address;
        int Port;
        string Login;
        string Password;
		public bool IsPing { get; set; }

        public OperationResult<bool> Connect(string address, int port, string login, string password, bool isPing)
        {
            Address = address;
            Port = port;
            Login = login;
            Password = password;
            IsPing = isPing;
            return DoConnect();
        }

        public OperationResult<bool> DoConnect()
        {
            IsConnected = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    StartConnectionTimeoutThread();

                    string loginError = null;

                    var result = GetConnection(Address, Port, Login, Password);
                    if (result.HasError &&
                        result.Error == "Пользователь или пароль неверны. Повторите ввод" ||
                        result.Error == "Удаленный доступ с этого компьютера запрещен")
                    {
                        loginError = result.Error;
                    }
                    Connection = result.Result;

                    if (loginError != null)
                    {
                        return new OperationResult<bool>("Неверный логин драйвера Firesec: " + loginError);
                    }
                    StopConnectionTimeoutThread();
                    if (Connection != null)
                        break;
                }
                if (Connection != null)
                    break;
                SocketServerHelper.Restart();
            }
            if (Connection == null)
            {
                LoadingErrorManager.Add("Ошибка при загрузке драйвера firesec");
                return new OperationResult<bool>("Ошибка при загрузке драйвера firesec");
            }

            IsConnected = true;
            SetLastEvent();
            return new OperationResult<bool>() { Result = true };
        }

        OperationResult<FS_Types.IFSC_Connection> GetConnection(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
        {
            try
            {
                SocketServerHelper.StartIfNotRunning();

                FS_Types.FSC_LIBRARY_CLASSClass library = new FS_Types.FSC_LIBRARY_CLASSClass();
                var serverInfo = new FS_Types.TFSC_ServerInfo()
                {
                    ServerName = FS_Address,
                    Port = FS_Port
                };

                try
                {
                    FS_Types.IFSC_Connection connectoin = library.Connect2(FS_Login, FS_Password, serverInfo, this);
                    return new OperationResult<FS_Types.IFSC_Connection>() { Result = connectoin };
                }
                catch (Exception e)
                {
                    Logger.Error("NativeFiresecClient.GetConnection " + e.Message);
                    return new OperationResult<FS_Types.IFSC_Connection>(e.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "NativeFiresecClient.GetConnection");
                return new OperationResult<FS_Types.IFSC_Connection>(e.Message);
            }
        }

        static Thread ConnectionTimeoutThread;
        static AutoResetEvent ConnectionTimeoutEvent;
        static void StartConnectionTimeoutThread()
        {
            ConnectionTimeoutEvent = new AutoResetEvent(false);
            ConnectionTimeoutThread = new Thread(OnConnectionTimeoutThread);
            ConnectionTimeoutThread.Start();
        }
        static void StopConnectionTimeoutThread()
        {
            if (ConnectionTimeoutEvent != null)
            {
                ConnectionTimeoutEvent.Set();
            }
            if (ConnectionTimeoutThread != null)
            {
                ConnectionTimeoutThread.Join(TimeSpan.FromSeconds(1));
            }
        }
        static void OnConnectionTimeoutThread()
        {
            if (ConnectionTimeoutEvent != null)
            {
                if (!ConnectionTimeoutEvent.WaitOne(TimeSpan.FromMinutes(3)))
                {
                    Logger.Error("NativeFiresecClient.OnConnectionTimeoutThread");
                    SocketServerHelper.Restart();
                }
            }
        }
    }
}