
namespace RubezhService
{
	static class StatusModel
	{
		public static string LocalAddress { get; private set; }
		public static string RemoteAddress { get; private set; }

		public static void SetLocalAddress(string address)
		{
			LocalAddress = address;
            // TODO: Notify
        }

        public static void SetRemoteAddress(string address)
		{
			RemoteAddress = address;
			// TODO: Notify View
		}
	}
}
