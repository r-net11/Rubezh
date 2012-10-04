namespace Infrastructure
{
	public class AppSettings
	{
        public string FS_Address { get; set; }
        public int FS_Port { get; set; }
        public string FS_Login { get; set; }
        public string FS_Password { get; set; }

		public string ServiceAddress { get; set; }
		public string LibVlcDllsPath { get; set; }
		public bool IsDebug { get; set; }
		public bool CanControl { get; set; }
		public bool HasLicenseToControl { get; set; }
        public string Theme { get; set; }
	}
}