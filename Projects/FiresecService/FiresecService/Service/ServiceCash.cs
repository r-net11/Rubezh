using System.Collections.Generic;
using System.Linq;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public static class ServiceCash
	{
		public static List<FiresecManager> FreeManagers { get; private set; }
		public static List<FiresecManager> RunningManagers { get; private set; }

		static ServiceCash()
		{
			FreeManagers = new List<FiresecManager>();
			RunningManagers = new List<FiresecManager>();
		}

		public static FiresecManager Get(FiresecService firesecService)
		{
			if (FreeManagers.Count > 0)
			{
				var freeManager = FreeManagers.First();
				freeManager.FiresecService = firesecService;
				RunningManagers.Add(freeManager);
				FreeManagers.Remove(freeManager);
				return freeManager;
			}
			else
			{
				var firesecManager = new FiresecManager(firesecService);
				RunningManagers.Add(firesecManager);
				return firesecManager;
			}
		}

		public static void Free(FiresecManager firesecManager)
		{
			//if (firesecManager.FiresecSerializedClient != null)
			//    firesecManager.FiresecSerializedClient.Disconnect();

			FreeManagers.Add(firesecManager);
			RunningManagers.Remove(firesecManager);
		}
	}
}