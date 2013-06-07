
using ServerFS2.Processor;
namespace ServerFS2
{
	public class WatcherManager
	{
		public static WatcherManager Current { get; private set; }

		public WatcherManager()
		{
			Current = this;
		}

		public void Start()
		{
			//MainManager.StartMonitoring();
			Bootstrapper.BootstrapperLoadEvent.Set();
		}

		public void Stop()
		{
			//MainManager.StopMonitoring();
		}
	}
}