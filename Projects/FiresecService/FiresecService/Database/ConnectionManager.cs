using FiresecService.DatabaseConverter;
using FiresecService.Properties;

namespace FiresecService.Database
{
	public static class ConnectionManager
	{
		public static FiresecDbConverterDataContext CreateFiresecDataContext()
		{
			return new FiresecDbConverterDataContext(Settings.Default.FiresecConnectionString);
		}
	}
}