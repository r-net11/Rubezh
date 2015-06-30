using System;

namespace Socktes
{
    public delegate void RecieveEventHandler(object Sender, RecieveEventArgs e);

    public class RecieveEventArgs : System.EventArgs
    {
        public RecieveEventArgs()
        {
        }

        internal int m_DataLength;
        internal byte[] m_bytesArray = new byte[Sockets.BufferSize.Size];
        internal IAsyncResult m_ar;

        public int Length
        {
            get
            {
                return m_DataLength;
            }
        }

        public byte[] Data
        {
            get
            {
                return m_bytesArray;
            }
        }

        public IAsyncResult AsyncResult
        {
            get
            {
                return m_ar;
            }
        }
    }
}