using System;

namespace Socktes
{
	public delegate void CloseEvenHandler(object Sender,CloseEventArgs e);
	/// <summary>
	/// 
	/// </summary>
	public class CloseEventArgs : System.EventArgs
	{
		public CloseEventArgs()
		{
			m_ar = null;
			m_SockEx = null;
			m_CloseArgument = "Not set yet";
		}

		internal void create (IAsyncResult ar,System.Net.Sockets.SocketException SockEx,string ClosArg)
		{
			m_ar = ar;
			m_SockEx = SockEx;
			m_CloseArgument = ClosArg;
		}
		private  IAsyncResult m_ar;
		private  System.Net.Sockets.SocketException m_SockEx;
		private string m_CloseArgument;
		public IAsyncResult Resulte
		{
			get 
			{
				return m_ar;
			}
		}

		public System.Net.Sockets.SocketException Exception 
		{
			get 
			{
				return m_SockEx;
			}
		}

		public string CloseArg
		{
			get
			{
				return  m_CloseArgument;
			}
		}


	}
}
