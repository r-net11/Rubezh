using System;
using System.Collections;
using System.IO;

namespace Socktes
{
	/// <summary>
	/// 
	/// </summary>
	public class ParserSocket : System.ComponentModel.Component
	{
		// vars 
		private ConnectSocket m_Socket;
		private MemoryStream m_buffer;
		protected int m_Position;
		private int m_Port;
		private string m_Host;
		protected object m_State; 

		//attributes 
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
				if(!m_Socket.SocketHandle.Connected)
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
				if(!m_Socket.SocketHandle.Connected)
					m_Host = value;
				else 
					throw new Exception("tring to change Host of a Connected Host");
			}
			get 
			{
				return m_Host;
			}
		}
		//operations 
		public void OpenConnection()
		{
			m_Socket.Connect(m_Host,m_Port);
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
			catch(System.Net.Sockets.SocketException Ex)
			{
				CloseEventArgs e = new CloseEventArgs();
				e.create(null,Ex,"Closed from the server");
				Disconnected(this,e);
			}
		}

		public event CloseEvenHandler Disconnected;
		public event MessageEndEventHandler MessageEnd;

		// inintilization 
		public ParserSocket()
		{
			m_buffer = new MemoryStream();
			m_Socket = new ConnectSocket();
			m_Port = 0;
			m_Host = "000.000.000.00";
			m_Position = 0;

			Disconnected +=new CloseEvenHandler(OnDisconnected);
			MessageEnd +=new MessageEndEventHandler(OnMessageEnd);

			m_Socket.recieve +=new RecieveEventHandler(m_Socket_recieve);
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
			MessageEnd(this,e);
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
			int j = e.Data.Length -1;
			for(int i = 0 ; i!= e.Length ; i++)
			{
				b[j] = 0;
				j--;
			}
			Parse(b);
		}


	}
}
