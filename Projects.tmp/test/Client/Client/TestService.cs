using System;
using System.ServiceModel;
using TestAPI;

namespace Client
{
	public class TestService : ITestService
	{
		public int ClientId { get; private set; }
		ChannelFactory<ITestService> _factory;

		public TestService(string address)
		{
			ClientId = new Random().Next(10000, 99999);
			var tcpUri = new Uri("net.tcp://" + address + "/TestService");
			var endpointAddress = new EndpointAddress(tcpUri);
			var binding = BindingHelper.CreateBinding();
			_factory = new ChannelFactory<ITestService>(binding, endpointAddress);
		}

		public void Void(int clientId)
		{
			var service = _factory.CreateChannel ();
			using (service as IDisposable)
				service.Void(clientId);
		}

		public void VoidOneWay(int clientId)
		{
			var service = _factory.CreateChannel ();
			using (service as IDisposable)
				service.VoidOneWay(clientId);
		}

		public int RandomInt(int clientId, int delay)
		{
			var service = _factory.CreateChannel ();
			using (service as IDisposable)
				return service.RandomInt(clientId, delay);
		}

		public OperationResult<bool> OperationResult(int clientId)
		{
			var service = _factory.CreateChannel ();
			using (service as IDisposable)
				return service.OperationResult(clientId);
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
