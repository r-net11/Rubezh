namespace FiresecAPI.Models
{
	public static class ConfigurationCash
	{
		static ConfigurationCash()
		{
			DriversConfiguration = new DriversConfiguration();
		}

		public static DriversConfiguration DriversConfiguration { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
	}
}