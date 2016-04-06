using System;
using System.ServiceModel;
using RubezhAPI;

namespace GKImitator
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class ImitatorService : IImitatorService
	{
		public OperationResult<string> TestImitator()
		{
			return new OperationResult<string>("Hello from Imitator");
		}
	}
}
