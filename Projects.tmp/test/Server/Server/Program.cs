using System;
using System.ServiceModel;
using TestAPI;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var address = args.Length == 0 ? "localhost:1050" : args [0];
			ServiceHost host = new ServiceHost(typeof(TestService), new Uri("net.tcp://" + address + "/TestService"));
			host.AddServiceEndpoint(typeof(ITestService), new NetTcpBinding(), "");
			host.Open();
			Console.WriteLine("Сервер запущен");
			Console.ReadLine();

			host.Close();
		}
	}
}
