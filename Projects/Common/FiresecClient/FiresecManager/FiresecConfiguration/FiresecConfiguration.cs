using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public GKDriversConfiguration DriversConfiguration
		{
			get { return GKConfigurationCash.GKDriversConfiguration; }
			set { GKConfigurationCash.GKDriversConfiguration = value; }
		}
	}
}