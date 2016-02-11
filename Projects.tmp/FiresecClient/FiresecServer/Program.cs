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
			var address = args.Length == 0 ? "192.168.21.70:9988" : args[0];
			Console.Title = address;
			ServiceHost host = new ServiceHost(typeof(FiresecService2));
			host.AddServiceEndpoint(typeof(IFiresecService), BindingHelper.CreateNetTcpBinding(), new Uri("net.tcp://" + address + "/FiresecService/"));

			host.Open();
			Console.WriteLine("Сервер запущен");
			Console.ReadLine();

			host.Close();
		}
	}
}
