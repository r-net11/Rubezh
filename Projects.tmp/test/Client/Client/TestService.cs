using System;
using System.ServiceModel;
using TestAPI;

namespace Client
{
	public class TestService : ITestService
	{
		public int ClientId { get; private set; }
		ITestService _service;

		public TestService(string address)
		{
			ClientId = new Random().Next(10000, 99999);
			var tcpUri = new Uri("net.tcp://" + address + "/TestService");
			var endpointAddress = new EndpointAddress(tcpUri);
			var binding = BindingHelper.CreateBinding();
			ChannelFactory<ITestService> factory = new ChannelFactory<ITestService>(binding, endpointAddress);
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

		public OperationResult<bool> OperationResult(int clientId)
		{
			return _service.OperationResult(clientId);
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

		public OperationResult<bool> OperationResult()
		{
			return OperationResult(ClientId);
		}

	}
}
