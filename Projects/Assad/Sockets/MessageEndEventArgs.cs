namespace Socktes
{
    public delegate void MessageEndEventHandler(object Sender, MessageEndEventArgs e);

    public class MessageEndEventArgs : System.EventArgs
    {
        internal object m_Type;
        internal object m_param;
        internal byte[] m_buff;

        public byte[] BufferSize
        {
            get
            {
                return m_buff;
            }
        }
        public object Type
        {
            get
            {
                return m_Type;
            }
        }

        public object Param
        {
            get
            {
                return m_param;
            }
        }
        public MessageEndEventArgs()
        {
            m_buff = null;
            m_param = null;
            m_Type = null;
        }
    }
}