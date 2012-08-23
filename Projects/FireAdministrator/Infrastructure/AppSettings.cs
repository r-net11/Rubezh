namespace Infrastructure
{
    public class AppSettings
    {
		public bool IsDebug { get; set; }
        public string ServiceAddress { get; set; }
        public bool ShowVideo { get; set; }
        public string LibVlcDllsPath { get; set; }
        public bool ShowGK { get; set; }
        public bool ShowSKUD { get; set; }
		public bool UseLocalConnection { get; set; }
        public string Theme { get; set; }
    }
}