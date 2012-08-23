namespace FiresecService
{
	public static class AppSettings
	{
		public static string OldFiresecLogin { get; set; }
		public static string OldFiresecPassword { get; set; }
		public static string ServiceAddress { get; set; }
		public static string LocalServiceAddress { get; set; }
		public static bool IsDebug { get; set; }
		public static bool DoNotOverrideFiresec1Config { get; set; }
        public static bool IsImitatorEnabled { get; set; }
        public static bool IsOPCEnabled { get; set; }
        public static bool IsFSEnabled { get; set; }
        public static bool IsGKEnabled { get; set; }
	}
}