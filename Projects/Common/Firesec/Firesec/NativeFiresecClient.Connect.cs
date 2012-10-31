using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        private int _reguestId = 1;
        private Dispatcher _dispatcher;

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
            dispatcherThread.Join(100);
        }

        FS_Types.IFSC_Connection Connection;
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
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    ConnectionTimeoutEvent = new AutoResetEvent(false);
                    ConnectionTimeoutThread = new Thread(new ThreadStart(OnConnectionTimeoutThread));
                    ConnectionTimeoutThread.Start();

                    _dispatcher.Invoke(
                    (
                        new Action
                        (
                            () =>
                            {
                                Connection = GetConnection(Address, Port, Login, Password);
                            }
                        )
                    ));
                    ConnectionTimeoutEvent.Set();
                    if (Connection != null)
                        break;
                }
                if (Connection != null)
                {
                    break;
                }
                else
                {
                    SocketServerHelper.Restart();
                }
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

            return new OperationResult<bool>() { Result = true };
        }

        static Thread ConnectionTimeoutThread;
        static AutoResetEvent ConnectionTimeoutEvent;
        static void OnConnectionTimeoutThread()
        {
            if (!ConnectionTimeoutEvent.WaitOne(TimeSpan.FromMinutes(2)))
            {
                Logger.Error("NativeFiresecClient.OnConnectionTimeoutThread");
                SocketServerHelper.Restart();
            }
        }

        FS_Types.IFSC_Connection GetConnection(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
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
                    return connectoin;
                }
                catch (Exception e)
                {
                    Logger.Error("NativeFiresecClient.GetConnection " + e.Message);
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "NativeFiresecClient.GetConnection");
                return null;
            }
        }
    }
}