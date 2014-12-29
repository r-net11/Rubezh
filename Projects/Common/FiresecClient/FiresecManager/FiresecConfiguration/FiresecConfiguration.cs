using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public GKDriversConfiguration XDriversConfiguration
		{
			get { return GKConfigurationCash.GKDriversConfiguration; }
			set { GKConfigurationCash.GKDriversConfiguration = value; }
		}
	}
}