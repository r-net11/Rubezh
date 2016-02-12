using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using TestAPI;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var address = args.Length == 0 ? "localhost:1050" : args[0];
			Console.Title = address;
			ServiceHost host = new ServiceHost(typeof(TestService), new Uri("net.tcp://" + address), new Uri("http://localhost:9987"));
			host.AddServiceEndpoint(typeof(ITestService), BindingHelper.CreateBinding(), "TestService");

			var smb = new ServiceMetadataBehavior();
			host.Description.Behaviors.Add(smb);
			var mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
			host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "mex");

			host.Open();

			//var throttle = ((ChannelDispatcher)host.ChannelDispatchers [0]).ServiceThrottle;
			//throttle.MaxConcurrentCalls = Int32.MaxValue;
			//throttle.MaxConcurrentInstances = Int32.MaxValue;
			//throttle.MaxConcurrentSessions = Int32.MaxValue;

			//Console.WriteLine("MaxConcurrentCalls = " + throttle.MaxConcurrentCalls);
			//Console.WriteLine("MaxConcurrentInstances = " + throttle.MaxConcurrentInstances);
			//Console.WriteLine("MaxConcurrentSessions = " + throttle.MaxConcurrentSessions);

			Console.WriteLine("Сервер запущен");
			Console.ReadLine();

			host.Close();
		}
	}
}
