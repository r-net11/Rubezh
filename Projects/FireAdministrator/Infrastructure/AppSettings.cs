namespace Infrastructure
{
    public class AppSettings
    {
        public string LibVlcDllsPath { get; set; }
		public bool IsDebug { get; set; }
		public bool DoNotOverrideFS1 { get; set; }
		public bool IsExpertMode { get; set; }
    }
}