namespace FiresecService
{
	public static class AppSettings
	{
		public static string ServiceAddress { get; set; }
		public static string LocalServiceAddress { get; set; }
		public static bool IsDebug { get; set; }
        public static bool IsImitatorEnabled { get; set; }
	}
}