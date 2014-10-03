namespace FiresecAPI.GK
{
	public static class XConfigurationCash
	{
		public static GKDriversConfiguration XDriversConfiguration { get; set; }
		static XConfigurationCash()
		{
			XDriversConfiguration = new GKDriversConfiguration();
		}
	}
}