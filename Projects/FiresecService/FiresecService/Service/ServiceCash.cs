using System.Collections.Generic;
using System.Linq;
using FiresecService.Processor;
using FiresecService.ViewModels;

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
			CallbackManager.Ping();

			FiresecManager firesecManager = null;

			if (FreeManagers.Count > 0)
			{
				firesecManager = FreeManagers.First();
				firesecManager.FiresecService = firesecService;
				RunningManagers.Add(firesecManager);
				FreeManagers.Remove(firesecManager);
			}
			else
			{
				firesecManager = new FiresecManager(firesecService);
				RunningManagers.Add(firesecManager);
			}

			MainViewModel.Current.UpdateComServersCount();
			return firesecManager;
		}

		public static void Free(FiresecManager firesecManager)
		{
			firesecManager.FiresecService = null;
			FreeManagers.Add(firesecManager);
			RunningManagers.Remove(firesecManager);
			MainViewModel.Current.UpdateComServersCount();
		}
	}
}