
namespace FiresecService
{
	static class StatusPresenter
	{
		public static string LocalAddress { get; private set; }
		public static string RemoteAddress { get; private set; }

		public static void SetLocalAddress(string address)
		{
			LocalAddress = address;
			PageController.OnPageChanged(Page.Status);
		}

		public static void SetRemoteAddress(string address)
		{
			RemoteAddress = address;
			PageController.OnPageChanged(Page.Status);
		}
	}
}
