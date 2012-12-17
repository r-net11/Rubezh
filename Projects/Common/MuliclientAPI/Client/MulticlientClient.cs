using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace MuliclientAPI
{
	public static class MulticlientClient
	{
		static DuplexChannelFactory<IMuliclient> ChannelFactory;
		static IMuliclientCallback MuliclientCallback;
		static string MulticlientClientId;
		public static IMuliclient Muliclient;
		static Thread PollThread;
		static AutoResetEvent StopPollEvent;

		public static void Start(IMuliclientCallback muliclientCallback, string multiclientClientId)
		{
			MuliclientCallback = muliclientCallback;
			MulticlientClientId = multiclientClientId;
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			var endpointAddress = new EndpointAddress(new Uri("net.pipe://localhost/MulticlientServer"));
			ChannelFactory = new DuplexChannelFactory<IMuliclient>(new InstanceContext(MuliclientCallback), binding, endpointAddress);
			ChannelFactory.Open();
			Muliclient = ChannelFactory.CreateChannel();

			PollThread = new Thread(OnRunPoll);
			PollThread.Start();
		}

		public static void Stop()
		{
			ChannelFactory.Close();
		}

		static void OnRunPoll()
		{
			while (true)
			{
				try
				{
					StopPollEvent = new AutoResetEvent(false);
					if(StopPollEvent.WaitOne(TimeSpan.FromSeconds(30)))
						break;

					Muliclient.SetStatus(MulticlientClientId, "Ping");
				}
				catch { }
			}
		}
	}
}