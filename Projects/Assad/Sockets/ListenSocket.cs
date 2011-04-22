using System;
using System.Net.Sockets;
using System.Net;

namespace Socktes
{
	/// <summary>
	/// 
	/// </summary>
	public class ListenSocket : System.ComponentModel.Component
	{
		public ListenSocket()
		{
			m_Socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			m_Port = 0;

			this.create +=new EventHandler(OnCreate);
			this.accept +=new AcceptEvenetHandler(OnAccept);
			this.close +=new CloseEvenHandler(OnClose);
		
		}

		//member variables 
		//private : 
		private IAsyncResult m_ar;
		private Socket m_Socket;
		private int m_Port;


		public int Port
		{
			set 
			{
				m_Port = value;
			}
			get 
			{
				return m_Port;
			}
		}
		public Socket SocketHandle 
		{
			set 
			{
				create(this,null);
				m_Socket = value;
			}
			get 
			{
				return m_Socket;
			}
		}
		
		// operations 
		public void StratListen(bool IsBlocked)
		{
			create(this,null);
			IPEndPoint ep = new IPEndPoint(IPAddress.Any,m_Port);
			m_Socket.Bind(ep);
			m_Socket.Listen(5);
			if(IsBlocked == false)
				m_Socket.BeginAccept(new AsyncCallback(OnSendEvents),null);

		}

		public void StopListen()
		{
			m_Socket.EndAccept(m_ar);
		}

		public Socket Accept()
		{
				Socket s = m_Socket.Accept();
				AcceptEventArgs e = new AcceptEventArgs();
				e.m_Socket = s;
				e.m_ar = null;
				accept(this,e);
				return s;
		}


		public void Close()
		{
			CloseEventArgs e = new CloseEventArgs();
			e.create(null,null,"Closed By user");
			close(this,e);
			m_Socket.Close();
		}

		public event System.EventHandler create;
		public event CloseEvenHandler close;
		public event AcceptEvenetHandler accept;

		private void OnSendEvents(IAsyncResult ar)
		{
			if(ar.IsCompleted)
			{
				Socket s = m_Socket.EndAccept(ar);
				AcceptEventArgs e = new AcceptEventArgs();
				e.m_ar = ar;
				m_ar = ar;
				e.m_Socket = s;
				accept(this,e);
				m_Socket.BeginAccept(new AsyncCallback(OnSendEvents),null);
			}
		}

		#region Protected Events
		protected void OnCreate(object sender, EventArgs e)
		{

		}

		protected void OnAccept(object Sender, AcceptEventArgs e)
		{
		}

		protected void OnClose(object Sender, CloseEventArgs e)
		{

		}
		#endregion 


	}
}
