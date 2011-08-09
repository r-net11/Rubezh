using System;
using System.Net.Sockets;

namespace Socktes
{
    public delegate void AcceptEvenetHandler(object Sender, AcceptEventArgs e);

    /// <summary>
    ///
    /// </summary>
    public class AcceptEventArgs : System.EventArgs
    {
        public AcceptEventArgs()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        internal Socket m_Socket;
        internal IAsyncResult m_ar;

        public Socket ConnectedSocket
        {
            get
            {
                return m_Socket;
            }
        }
        public IAsyncResult Result
        {
            get
            {
                return m_ar;
            }
        }
    }
}