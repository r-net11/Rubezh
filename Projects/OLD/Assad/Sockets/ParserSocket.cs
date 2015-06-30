using System;
using System.IO;

namespace Socktes
{
    public class ParserSocket : System.ComponentModel.Component
    {
        private ConnectSocket m_Socket;
        private MemoryStream m_buffer;
        protected int m_Position;
        private int m_Port;
        private string m_Host;
        protected object m_State;

        public ConnectSocket ConnectSocket
        {
            get
            {
                return m_Socket;
            }
        }

        public int Position
        {
            get
            {
                return m_Position;
            }
        }

        public int Port
        {
            set
            {
                if (!m_Socket.SocketHandle.Connected)
                    m_Port = value;
                else
                    throw new Exception("tring to change Port of a Connected socket");
            }
            get
            {
                return m_Port;
            }
        }

        public string Host
        {
            set
            {
                if (!m_Socket.SocketHandle.Connected)
                    m_Host = value;
                else
                    throw new Exception("tring to change Host of a Connected Host");
            }
            get
            {
                return m_Host;
            }
        }

        public void OpenConnection()
        {
            m_Socket.Connect(m_Host, m_Port);
        }

        public void CloseConnection()
        {
            m_Socket.Close();
        }

        public void Send(byte[] buff)
        {
            try
            {
                m_Socket.Send(buff);
            }
            catch (System.Net.Sockets.SocketException Ex)
            {
                CloseEventArgs e = new CloseEventArgs();
                e.create(null, Ex, "Closed from the server");
                Disconnected(this, e);
            }
        }

        public event CloseEvenHandler Disconnected;
        public event MessageEndEventHandler MessageEnd;


        public ParserSocket()
        {
            m_buffer = new MemoryStream();
            m_Socket = new ConnectSocket();
            m_Port = 0;
            m_Host = "000.000.000.00";
            m_Position = 0;

            Disconnected += new CloseEvenHandler(OnDisconnected);
            MessageEnd += new MessageEndEventHandler(OnMessageEnd);

            m_Socket.recieve += new RecieveEventHandler(m_Socket_recieve);
        }

        //overwrides
        /// <summary>
        /// called by the framework when recieving data from the remote computer
        /// this method must be overwride.
        /// use this function to concatinate many recieved data into one message
        /// when the message end fire the event MessageEnd().
        /// </summary>
        /// <param name="buff"></param>
        protected void Parse(byte[] buff)
        {
            MessageEndEventArgs e = new MessageEndEventArgs();
            e.m_buff = buff;
            MessageEnd(this, e);
        }

        protected void OnDisconnected(object Sender, CloseEventArgs e)
        {
        }

        protected void OnMessageEnd(object Sender, MessageEndEventArgs e)
        {
        }

        private void m_Socket_recieve(object Sender, RecieveEventArgs e)
        {
            byte[] b = e.Data;
            for (int i = 0; i < b.Length; ++i)
            {
                b[i] = 0;
            }

            Parse(b);
        }
    }
}