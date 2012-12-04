using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace TestWebServer
{
	public static class ServerHost
	{
		static ServiceHost ServiceHost;
		static TestContract TestContract;

		public static void Run()
		{
			TestContract = new TestContract();
			ServiceHost = new ServiceHost(TestContract);

			var address = "http://localhost:9000/FSTest/";
			ServiceHost.AddServiceEndpoint("TestWebServer.ITestContract", CreateWSHttpBinding(), new Uri(address));

			var mexAddress = "http://localhost:9000/FSTestMex/";
			ServiceMetadataBehavior metadataBehavior = ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metadataBehavior == null)
			{
				metadataBehavior = new ServiceMetadataBehavior();
				ServiceHost.Description.Behaviors.Add(metadataBehavior);
			}
			ServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), CreateWSHttpBinding(), mexAddress);

			ServiceHost.Open();
		}

		static WSHttpBinding CreateWSHttpBinding()
		{
			var binding = new WSHttpBinding(SecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			return binding;
		}
	}
}