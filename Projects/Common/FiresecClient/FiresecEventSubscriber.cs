using System.ServiceModel;
using FiresecAPI;

namespace FiresecClient
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
	public class FiresecEventSubscriber : IFiresecCallback
	{
		public void Ping()
		{
		}
	}
}