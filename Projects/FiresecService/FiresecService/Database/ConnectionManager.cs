using FiresecService.Properties;
using FiresecService.Database;
using FiresecService.DatabaseConverter;

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