using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MuliclientAPI
{
	public static class MulticlientServer
	{
		static ServiceHost ServiceHost;
		public static Muliclient Muliclient { get; set; }

		public static void Start()
		{
			if (ServiceHost != null)
				return;

			Muliclient = new Muliclient();
			ServiceHost = new ServiceHost(Muliclient);
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			ServiceHost.AddServiceEndpoint("MuliclientAPI.IMuliclient", binding, new Uri("net.pipe://localhost/MulticlientServer"));

			ServiceHost.Open();
		}

		public static void Stop()
		{
			ServiceHost.Close();
			ServiceHost = null;
		}
	}
}