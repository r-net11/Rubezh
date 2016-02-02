using System;
using System.ServiceModel;
using TestAPI;

namespace Client
{
	public class TestService : ITestService
	{
		public int ClientId { get; private set; }
		ITestService _service;

		public TestService()
		{
			ClientId = new Random().Next(10000, 99999);
			var tcpUri = new Uri("net.tcp://localhost:1050/TestService");
			var address = new EndpointAddress(tcpUri);
			var binding = new NetTcpBinding();
			ChannelFactory<ITestService> factory = new ChannelFactory<ITestService>(binding, address);
			_service = factory.CreateChannel();
		}

		public void Void(int clientId)
		{
			_service.Void(clientId);
		}

		public void VoidOneWay(int clientId)
		{
			_service.VoidOneWay(clientId);
		}

		public int RandomInt(int clientId, int delay)
		{
			return _service.RandomInt(clientId, delay);
		}

		public void Void()
		{
			Void(ClientId);
		}

		public void VoidOneWay()
		{
			VoidOneWay(ClientId);
		}

		public int RandomInt(int delay)
		{
			return RandomInt(ClientId, delay);
		}
	}
}
