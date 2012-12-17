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
		static ServiceHost SelfHost;

		public static void Run()
		{
			BasicHttpBinding myBinding = new BasicHttpBinding();
			myBinding.Security.Mode = BasicHttpSecurityMode.None;

			Uri baseAddress = new Uri("http://localhost:9000/FSTest/");
			SelfHost = new ServiceHost(typeof(TestContract), baseAddress);

			SelfHost.AddServiceEndpoint(typeof(ITestContract), myBinding, "CoreServices");

			ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
			smb.HttpGetEnabled = true;
			smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy12;
			SelfHost.Description.Behaviors.Add(smb);

			SelfHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

			SelfHost.Open();
		}
	}
}