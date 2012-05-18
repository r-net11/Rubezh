using System.Collections.Generic;
using System.Linq;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public static class ServiceCash
	{
		static List<FiresecManager> FreeManagers { get; set; }

		static ServiceCash()
		{
			FreeManagers = new List<FiresecManager>();
		}

		public static FiresecManager Get(FiresecService firesecService)
		{
			if (FreeManagers.Count > 0)
			{
				var freeManager = FreeManagers.First();
				freeManager.FiresecService = firesecService;
				return freeManager;
			}
			else
			{
				return new FiresecManager(firesecService);
			}
		}

		public static void Free(FiresecManager firesecManager)
		{
			//if (firesecManager.FiresecSerializedClient != null)
			//    firesecManager.FiresecSerializedClient.Disconnect();

			FreeManagers.Add(firesecManager);
		}
	}
}