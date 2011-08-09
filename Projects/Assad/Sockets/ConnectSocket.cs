using System;
using System.Net;
using System.Net.Sockets;

namespace Socktes
{
    /// <summary>
    /// Connection Mode, how the connect socket connect and recieve data from the remote end point
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        ///connect and wait untill connection completed.you must call recieve and Recieve data, the recieve event will not fire even if the remote host send data.
        ///return after the connection complete
        ///<seealso cref="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/cpref/html/frlrfsystemnetsocketssocketclassconnecttopic.htm"/>
        /// </summary>
        SyncConnectSyncRecv,
        /// <summary>
        /// (recomended)
        /// connect and wait untill connection compledted and begin wait for data.
        /// return after connection completed.
        /// </summary>
        SyncConnectAsyncRecv,
        /// <summary>
        ///  (not recomended)
        ///  begin connect and return emidiatly you must get the connect event to get connection .
        ///  you must call Recieve method to get data
        ///  <seealso cref="ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/cpref/html/frlrfsystemnetsocketssocketclassbeginconnecttopic.htm"/>
        /// </summary>
        AsyncConnectSyncRecv,
        /// <summary>
        /// begin connect and return.
        /// begin wait to get data.
        /// you must get the recieve event to get data
        /// </summary>
        AsyncConnectAsyncRecv
    }

    /// <summary>
    /// Control used as a socket that have the ability to create a connection, Recieve and send data to remote computer
    /// </summary>
    public class ConnectSocket : System.ComponentModel.Component
    {
        // data members
        // private :
        /// <summary>
        /// Socket handler
        /// </summary>
        private Socket m_Socket;
        /// <summary>
        /// the block state, true : bloked , false : not blocked
        /// </summary>
        private bool m_BlockingMode;

        //attributes
        //public :

        /// <summary>
        /// Bind the socket to a local IPEndPoint
        /// </summary>
        /// <param name="Ep">the EndPoint to bound to</param>
        public void Bind(IPEndPoint Ep)
        {
            m_Socket.Bind(Ep);
        }

        /// <summary>
        ///  Bind the socket to a local IPEndPoint
        /// </summary>
        /// <param name="IP">represent a doted number</param>
        /// <param name="Port">represent the port to be bound to</param>
        public void Bind(string IP, int Port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), Port);
            m_Socket.Bind(ep);
        }

        /// <summary>
        /// If the socket is in blocking mode
        /// true: blocked, false: not blocked
        /// </summary>
        public bool IsBlocked
        {
            set
            {
                m_BlockingMode = value;
                m_Socket.Blocking = value;

                if ((m_Socket.Connected) && (!m_BlockingMode))
                {
                    RecieveEventArgs rv = new RecieveEventArgs();
                    rv.m_DataLength = Sockets.BufferSize.Size;
                    m_Socket.ReceiveBufferSize = Sockets.BufferSize.Size;
                    m_Socket.BeginReceive(rv.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), rv);
                    return;
                }
            }
            get
            {
                return m_BlockingMode;
            }
        }

        /// <summary>
        /// Socket class that is the main object of the class
        /// return the Socket instatce of this object
        /// set the Socket instance of value passed to and change the blocking mode if needed
        /// </summary>
        public Socket SocketHandle
        {
            get
            {
                return m_Socket;
            }
            set
            {
                m_Socket = value;
                m_BlockingMode = value.Blocking;
                if (value.Connected)
                {
                    RecieveEventArgs e = new RecieveEventArgs();
                    e.m_DataLength = Sockets.BufferSize.Size;
                    m_Socket.ReceiveBufferSize = Sockets.BufferSize.Size;
                    m_Socket.BeginReceive(e.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), e);
                }
            }
        }

        /// <summary>
        /// the end point that is connected to
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return (IPEndPoint) m_Socket.RemoteEndPoint;
            }
        }

        /// <summary>
        /// the end point that is bound to
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint) m_Socket.LocalEndPoint;
            }
        }

        //initialization
        //public :
        public ConnectSocket()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_BlockingMode = true;

            close += new CloseEvenHandler(OnClose);
            connect += new EventHandler(OnConnect);
            create += new EventHandler(OnCreate);
            recieve += new RecieveEventHandler(OnRecieve);
            send += new EventHandler(OnSend);
            shutdown += new EventHandler(OnShutdown);
        }

        //operations
        // public :
        /// <summary>
        /// connect to a spacific Host and port
        /// </summary>
        /// <param name="Host">the host to connect can be either a doted IP or a host name </param>
        /// <param name="port">port to connect to</param>
        public void Connect(string Host, int port)
        {
            create(this, null);

            IPHostEntry he = Dns.Resolve(Host);
            IPEndPoint ep = new IPEndPoint(he.AddressList[0], port);

            if (m_BlockingMode)
            {
                m_Socket.Connect(ep);
                return;
            }

            m_Socket.BeginConnect(ep, new AsyncCallback(OnSendEvents), "connect");
            RecieveEventArgs rv = new RecieveEventArgs();
            rv.m_DataLength = Sockets.BufferSize.Size;
            m_Socket.ReceiveBufferSize = Sockets.BufferSize.Size;
            m_Socket.BeginReceive(rv.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), rv);
        }

        /// <summary>
        /// connect to a spacific Host and port
        /// </summary>
        /// <param name="Host">the host to connect can be either a doted IP or a host name </param>
        /// <param name="port">port to connect to</param>
        /// <param name="Mode">one of the ConnectionMode enum, use SyncConnetionAsycRecv as recommanded option</param>
        public void Connect(string Host, int port, ConnectionMode Mode)
        {
            create(this, null);

            IPHostEntry he = Dns.Resolve(Host);
            IPEndPoint ep = new IPEndPoint(he.AddressList[0], port);
            RecieveEventArgs rv = new RecieveEventArgs();

            switch (Mode)
            {
                case ConnectionMode.AsyncConnectAsyncRecv:
                    m_Socket.BeginConnect(ep, new AsyncCallback(OnSendEvents), "connect");
                    rv.m_DataLength = Sockets.BufferSize.Size;
                    m_Socket.ReceiveBufferSize = Sockets.BufferSize.Size;
                    m_Socket.BeginReceive(rv.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), rv);
                    return;
                case ConnectionMode.AsyncConnectSyncRecv:
                    m_Socket.BeginConnect(ep, new AsyncCallback(OnSendEvents), "connect");
                    return;
                case ConnectionMode.SyncConnectAsyncRecv:
                    m_Socket.Connect(ep);
                    rv.m_DataLength = Sockets.BufferSize.Size;
                    m_Socket.ReceiveBufferSize = Sockets.BufferSize.Size;
                    m_Socket.BeginReceive(rv.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), rv);
                    return;
                case ConnectionMode.SyncConnectSyncRecv:
                    m_Socket.Connect(ep);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// send the arry of bytes to the remote end point
        /// </summary>
        /// <param name="buff">arry of bytes to send</param>
        public void Send(byte[] buff)
        {
            if (m_BlockingMode)
            {
                m_Socket.Send(buff);
                return;
            }
            m_Socket.BeginSend(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(OnSendEvents), "send");
        }

        /// <summary>
        /// recieve an array of bytes from the remote end point
        /// the system will block until the socket recive data
        /// </summary>
        /// <param name="Length">the maximum length of the array of data </param>
        /// <returns>the array of bytes to be recieved</returns>
        public byte[] Recieve(int Length)
        {
            byte[] buff = new byte[Length];
            int length = m_Socket.Receive(buff);
            Array.Copy(buff, 0, buff, 0, length);
            return buff;
        }

        /// <summary>
        /// close the socket
        /// each socket created must be closed
        /// </summary>
        public void Close()
        {
            CloseEventArgs e = new CloseEventArgs();
            e.create(null, null, "closed by the user");
            close(this, e);
            m_Socket.Close();
        }

        /// <summary>
        /// disable send or recieve or both options for this connection
        /// </summary>
        /// <param name="sh">the way of blocking </param>
        public void ShutDown(SocketShutdown sh)
        {
            shutdown(this, null);
            m_Socket.Shutdown(sh);
        }

        /// <summary>
        /// fire when a connection is created or changed
        /// </summary>
        public event System.EventHandler create;
        /// <summary>
        /// fire when a connection is closed by the user or by the remote server
        /// </summary>
        public event CloseEvenHandler close;
        /// <summary>
        /// fire when the user call shutdown method
        /// </summary>
        public event System.EventHandler shutdown;
        /// <summary>
        /// fire when data recieved from the a remote server
        /// fire in both mode , blocking or not blocking
        /// </summary>
        public event RecieveEventHandler recieve;
        /// <summary>
        /// fire when connection completed in not Blocking mode
        /// </summary>
        public event System.EventHandler connect;

        /// <summary>
        /// fire befor sending data to remote server
        /// </summary>
        public event System.EventHandler send;

        //AsyncHandlid code
        //private :
        /// <summary>
        /// Fire Socket Events
        /// called by the Class when events happend
        /// </summary>
        /// <param name="ar">the Asyc result </param>
        void OnSendEvents(IAsyncResult ar)
        {
            string s = ar.AsyncState.ToString();

            switch (s)
            {
                case "connect":
                    connect(this, null);
                    if (ar.IsCompleted)
                        m_Socket.EndConnect(ar);
                    break;
                case "send":
                    if (ar.IsCompleted)
                        m_Socket.EndSend(ar);
                    send(this, null);
                    break;
                default:
                    RecieveEventArgs e = (RecieveEventArgs) ar.AsyncState;
                    e.m_ar = ar;
                    recieve(this, e);
                    break;
            }
        }

        #region Protected events

        protected void OnConnect(object sender, EventArgs e)
        {
        }

        protected void OnCreate(object sender, EventArgs e)
        {
        }

        protected void OnRecieve(object Sender, RecieveEventArgs e)
        {
            try
            {
                e.m_DataLength = m_Socket.EndReceive(e.AsyncResult);
                RecieveEventArgs ev = new RecieveEventArgs();
                m_Socket.BeginReceive(ev.m_bytesArray, 0, Sockets.BufferSize.Size, SocketFlags.None, new AsyncCallback(OnSendEvents), ev);
            }
            catch (SocketException er)
            {
                CloseEventArgs c = new CloseEventArgs();
                c.create(e.AsyncResult, er, "Connection closed from the Server");
                close(this, c);
            }
        }

        protected void OnSend(object sender, EventArgs e)
        {
        }

        protected void OnShutdown(object sender, EventArgs e)
        {
        }

        protected void OnClose(object Sender, CloseEventArgs e)
        {
        }

        #endregion
    }
}