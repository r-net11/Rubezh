using System;
using System.ServiceModel;
using TestAPI;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost host = new ServiceHost(typeof(TestService), new Uri("net.tcp://localhost:1050/TestService"));
			host.AddServiceEndpoint(typeof(ITestService), new NetTcpBinding(), "");
			host.Open();
			Console.WriteLine("Сервер запущен");
			Console.ReadLine();

			host.Close();
		}
	}
}
