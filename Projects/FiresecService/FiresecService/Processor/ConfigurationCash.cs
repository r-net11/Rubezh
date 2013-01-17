using FiresecAPI.Models;
using FiresecService.Processor;

namespace FiresecService
{
	public static class ConfigurationCash
	{
		public static SecurityConfiguration SecurityConfiguration { get; set; }

		static ConfigurationCash()
		{
			SecurityConfiguration = SecurityConfigurationHelper.GetSecurityConfiguration();
		}
	}
}