using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Timers;
using System.Threading;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		bool isConnected = true;
		static AutoResetEvent StopPingEvent;
		static Thread pingThread;
		static bool suspendPing = false;

		public void StartPing()
		{
			StopPingEvent = new AutoResetEvent(false);
			pingThread = new Thread(new ThreadStart(OnPing));
			pingThread.Start();
		}

		public void StopPing()
		{
			if (StopPingEvent != null)
			{
				StopPingEvent.Set();
			}
		}

		void OnPing()
		{
			while (true)
			{
				if (!suspendPing)
				{
					Ping();
				}
				if (StopPingEvent.WaitOne(10000))
				{
					break;
				}
			}
		}

		public string Ping()
		{
			try
			{
				var result = FiresecService.Ping();
				OnConnectionAppeared();
				return result;
			}
			catch
			{
				OnConnectionLost();
				Recover();
			}
			return null;
		}

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (isConnected == false)
				return;

			if (ConnectionLost != null)
				ConnectionLost();

			isConnected = false;
		}

		public static event Action ConnectionAppeared;
		void OnConnectionAppeared()
		{
			if (isConnected == true)
				return;

			if (ConnectionAppeared != null)
				ConnectionAppeared();

			isConnected = true;
		}

		bool Recover()
		{
			suspendPing = true;
			FiresecServiceFactory.Dispose();
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			FiresecService = FiresecServiceFactory.Create(_serverAddress);
			try
			{
				FiresecService.Connect(_clientCredentials, false);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				suspendPing = false;
			}
		}
	}
}