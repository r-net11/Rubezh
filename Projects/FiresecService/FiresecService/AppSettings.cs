namespace FiresecService
{
	public static class AppSettings
	{
		public static string OldFiresecLogin { get; set; }
		public static string OldFiresecPassword { get; set; }
		public static string ServiceAddress { get; set; }
		public static string LocalServiceAddress { get; set; }
		public static bool IsDebug { get; set; }
		public static bool OverrideFiresec1Config { get; set; }
		public static bool IsImitatorVisible { get; set; }
		public static bool RunOPC { get; set; }
	}
}