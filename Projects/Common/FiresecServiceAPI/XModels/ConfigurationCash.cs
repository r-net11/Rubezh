namespace FiresecAPI.GK
{
	public static class XConfigurationCash
	{
		public static XDriversConfiguration XDriversConfiguration { get; set; }
		static XConfigurationCash()
		{
			XDriversConfiguration = new XDriversConfiguration();
		}
	}
}