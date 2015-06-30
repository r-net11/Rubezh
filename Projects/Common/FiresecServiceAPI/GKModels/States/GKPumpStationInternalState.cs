namespace FiresecAPI.GK
{
	public class GKPumpStationInternalState : GKBaseInternalState
	{
		public GKPumpStation PumpStation { get; set; }

		public GKPumpStationInternalState(GKPumpStation pumpStation)
		{
			PumpStation = pumpStation;
		}
	}
}