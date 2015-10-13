namespace RubezhAPI.GK
{
	public class GKPimInternalState : GKBaseInternalState
	{
		public GKPim Pim { get; set; }

		public GKPimInternalState(GKPim pim)
		{
			Pim = pim;
		}
	}
}