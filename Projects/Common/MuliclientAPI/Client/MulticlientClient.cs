using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MuliclientAPI
{
	public static class MulticlientClient
	{
		static DuplexChannelFactory<IMuliclient> ChannelFactory;
		static MuliclientCallback MuliclientCallback;
		public static IMuliclient Muliclient;

		public static void Start()
		{
			MuliclientCallback = new MuliclientCallback();

			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			var endpointAddress = new EndpointAddress(new Uri("net.pipe://localhost/MulticlientServer"));
			ChannelFactory = new DuplexChannelFactory<IMuliclient>(new InstanceContext(MuliclientCallback), binding, endpointAddress);

			ChannelFactory.Open();

			Muliclient = ChannelFactory.CreateChannel();
		}

		public static void Stop()
		{
			ChannelFactory.Close();
		}
	}
}