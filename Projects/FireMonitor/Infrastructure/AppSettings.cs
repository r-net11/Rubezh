namespace Infrastructure
{
	public class AppSettings
	{
		public string LibVlcDllsPath { get; set; }
		public bool CanControl { get; set; }
		public bool HasLicenseToControl { get; set; }
        public string Theme { get; set; }
	}
}