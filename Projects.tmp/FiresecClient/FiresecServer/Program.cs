using Common;
using RubezhAPI;
using System;
using System.ServiceModel;

namespace FiresecServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var address = args.Length == 0 ? "localhost:9988" : args[0];
			Console.Title = address;
			ServiceHost host = new ServiceHost(typeof(FiresecService2), new Uri("net.tcp://" + address));
			host.AddServiceEndpoint(typeof(IFiresecService), BindingHelper.CreateNetTcpBinding(), "FiresecService");

			host.Open();
			Console.WriteLine("Сервер запущен");
			Console.ReadLine();

			host.Close();
		}
	}
}
