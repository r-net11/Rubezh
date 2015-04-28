using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Xml.Serialization;

namespace Main
{
    public class NetManager
    {
        public Thread thread;
        public TcpListener tcpListener;
        public Socket socket;
        public int localPort = 2002;
        public int remotePort = 2002;
        public string udpBroadcastMessage = "<processor><host port=\"2002\" /><state idle=\"true\" /></processor>";
        MessageProcessor messageProcessor;
        byte[] recievedBytes = new byte[102400];
        int messageNo = 0;

        Socktes.ListenSocket Socket_Listen;
        Socktes.ConnectSocket Socket_Connection;

        #region static members
        static NetManager tcpServer;
        public static void Send(object confirmation, string refMessageId)
        {
            tcpServer.send(confirmation, refMessageId);
        }
        #endregion

        public NetManager()
        {
            tcpServer = this;
            messageProcessor = new MessageProcessor();
        }

        public void Start()
        {
            Socket_Listen = new Socktes.ListenSocket();
            Socket_Listen.Port = localPort;
            Socket_Listen.accept += new Socktes.AcceptEvenetHandler(this.Socket_Listen_accept);

            Socket_Connection = new Socktes.ConnectSocket();
            Socket_Connection.IsBlocked = false;
            Socket_Connection.recieve += new Socktes.RecieveEventHandler(this.Socket_Connection_recieve);

            Socket_Listen.StratListen(false);
        }

        private void Socket_Listen_accept(object Sender, Socktes.AcceptEventArgs e)
        {
            Socket_Connection.SocketHandle = e.ConnectedSocket;
        }

        private void Socket_Connection_recieve(object Sender, Socktes.RecieveEventArgs e)
        {
            string message;
            message = Encoding.UTF8.GetString(e.Data).Trim();

            int endIndex = message.IndexOf("\0");
            if (endIndex > 0)
                message = message.Remove(message.IndexOf("\0"));

            Logger.Logger.PrintIncomming(e.Data);
            if (message != "")
                messageProcessor.ProcessMessage(message);
        }

        object locker = new object();

        void send(object confirmation, string refMessageId)
        {
            lock (locker)
            {
                messageNo++;

                x.MessageType messageType = new x.MessageType();
                messageType.msgTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                messageType.msgId = "messageNo." + messageNo.ToString();
                if (!String.IsNullOrEmpty(refMessageId))
                    messageType.refMsg = refMessageId;
                messageType.Item = confirmation;

                XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, messageType);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                string message = Encoding.UTF8.GetString(bytes);
                message = RemoveWhitespaces(message);
                bytes = Encoding.UTF8.GetBytes(message);

                Socket_Connection.Send(bytes);
                Logger.Logger.PrintOutgoing(bytes);
            }
        }

        public string RemoveWhitespaces(string message)
        {
            message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", "");
            message = message.Replace("\r\n", "");

            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");

            return message;
        }

        public void Stop()
        {
            try
            {
                Socket_Connection.ShutDown(SocketShutdown.Both);
                Socket_Listen.StopListen();
            }
            catch
            {
            }
        }

        public void SendBroadcastUdp()
        {
            UdpClient udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);

            byte[] sendBytes = Encoding.UTF8.GetBytes(udpBroadcastMessage);

            udpClient.Send(sendBytes, sendBytes.Length, endPoint);

            udpClient.Close();
        }
    }
}
