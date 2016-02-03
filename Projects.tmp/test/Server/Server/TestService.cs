using System;
using System.ServiceModel;
using System.Threading;
using TestAPI;
namespace Server
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class TestService : TestAPI.ITestService
	{
		public void Void(int clientId)
		{
			Console.WriteLine("{0}: Void", clientId);
			Thread.Sleep(10000);
		}

		public void VoidOneWay(int clientId)
		{
			Console.WriteLine("{0}: VoidOneWay", clientId);
			Thread.Sleep(1000);
		}

		public int RandomInt(int clientId, int delay)
		{
			Console.WriteLine("{0}: RandomInt({1})", clientId, delay);
			Thread.Sleep(delay);
			return new Random().Next(100, 999);
		}
		public OperationResult<bool> OperationResult(int clientId)
		{
			Console.WriteLine("{0}: OperationResult<bool>", clientId);
			return new OperationResult<bool>(true);
		}
	}
}
