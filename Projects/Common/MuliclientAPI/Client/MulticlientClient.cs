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
		static IMuliclientCallback MuliclientCallback;
		public static IMuliclient Muliclient;

        public static void Start(IMuliclientCallback muliclientCallback)
		{
            MuliclientCallback = muliclientCallback;

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