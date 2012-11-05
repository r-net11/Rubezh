using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        Dispatcher _dispatcher;
        int _reguestId = 1;

        public NativeFiresecClient()
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
            {
                if (_dispatcher != null)
                {
                    if (Connection != null)
                    {
                        StopLifetimeThread();
                        StopPingThread();
                        StopOperationQueueThread();
                        StopConnectionTimeoutThread();
                        Marshal.FinalReleaseComObject(Connection);
                        Connection = null;
                        GC.Collect();
                    }
                    _dispatcher.InvokeShutdown();
                }
            };
            var dispatcherThread = new Thread(new ParameterizedThreadStart(obj =>
            {
                _dispatcher = Dispatcher.CurrentDispatcher;
                Dispatcher.Run();
                Debug.WriteLine("Native Dispatcher Stopped");
            }));
            dispatcherThread.SetApartmentState(ApartmentState.STA);
            dispatcherThread.IsBackground = true;
            dispatcherThread.Start();
            dispatcherThread.Join(TimeSpan.FromSeconds(1));
        }

        FS_Types.IFSC_Connection Connection;
        bool IsConnected = false;
        string Address;
        int Port;
        string Login;
        string Password;
        bool IsPing;

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

                    bool badLogin = false;
                    _dispatcher.Invoke(
                    (
                        new Action
                        (
                            () =>
                            {
                                var result = GetConnection(Address, Port, Login, Password);
                                if (result.HasError && result.Error == "Пользователь или пароль неверны. Повторите ввод")
                                {
                                    badLogin = true;
                                }
                                Connection = result.Result;
                            }
                        )
                    ));
                    if (badLogin)
                    {
                        return new OperationResult<bool>("Неверный логин драйвера Firesec");
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
                FiresecDriver.LoadingErrors.Append("Ошибка при загрузке драйвера firesec");
                return new OperationResult<bool>("Ошибка при загрузке драйвера firesec");
            }

            if (IsPing)
            {
                StartPingThread();
            }
            StartLifetimeThread();
            StartOperationQueueThread();
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
                if (!ConnectionTimeoutEvent.WaitOne(TimeSpan.FromMinutes(2)))
                {
                    Logger.Error("NativeFiresecClient.OnConnectionTimeoutThread");
                    SocketServerHelper.Restart();
                }
            }
        }
    }
}