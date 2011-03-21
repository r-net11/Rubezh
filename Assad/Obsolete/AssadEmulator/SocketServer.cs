using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadEmulator
{
    class SocketServer
    {
        static Socktes.ConnectSocket Socket_Connection;

        static bool connected = false;
        public static bool Connected
        {
            get { return connected; }
        }

        public static void Connect()
        {
            Socket_Connection = new Socktes.ConnectSocket();
            Socket_Connection.IsBlocked = false;
            Socket_Connection.recieve += new Socktes.RecieveEventHandler(Socket_Connection_recieve);
            Socket_Connection.Connect("localhost", 2002);
            connected = true;
        }

        static void Socket_Connection_recieve(object Sender, Socktes.RecieveEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Data).Trim();
            int endIndex = message.IndexOf("\0");
            if (endIndex > 0)
                message = message.Remove(message.IndexOf("\0"));

            OnRecieved(message);
        }

        public static void Send(string message)
        {
            Socket_Connection.Send(Encoding.UTF8.GetBytes(message));
        }

        public static void Stop()
        {
            Socket_Connection.Close();
        }

        static void OnRecieved(string messane)
        {
            if (Recieved != null)
                Recieved(messane);
        }
        public static event Action<string> Recieved;
    }
}
