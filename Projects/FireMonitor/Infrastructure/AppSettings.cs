namespace Infrastructure
{
    public class AppSettings
    {
        public string ServiceAddress { get; set; }
        public string DefaultLogin { get; set; }
        public string DefaultPassword { get; set; }
        public bool AutoConnect { get; set; }
        public string LibVlcDllsPath { get; set; }
        public bool ShowOnlyVideo { get; set; }
        public bool IsDebug { get; set; }
        public bool CanControl { get; set; }
        public bool ShowGK { get; set; }
    }
}