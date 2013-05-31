
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
			Bootstrapper.BootstrapperLoadEvent.Set();
		}

		public void Stop()
		{
		}
	}
}