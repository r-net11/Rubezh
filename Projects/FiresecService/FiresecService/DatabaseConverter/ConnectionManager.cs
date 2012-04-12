using FiresecService.Properties;

namespace FiresecService.DatabaseConverter
{
    public static class ConnectionManager
    {
        public static FiresecDbConverterDataContext CreateFiresecDataContext()
        {
            return new FiresecDbConverterDataContext(Settings.Default.FiresecConnectionString);
        }
    }
}