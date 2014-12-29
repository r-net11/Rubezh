namespace FiresecAPI.GK
{
	public static class GKConfigurationCash
	{
		public static GKDriversConfiguration GKDriversConfiguration { get; set; }
		static GKConfigurationCash()
		{
			GKDriversConfiguration = new GKDriversConfiguration();
		}
	}
}