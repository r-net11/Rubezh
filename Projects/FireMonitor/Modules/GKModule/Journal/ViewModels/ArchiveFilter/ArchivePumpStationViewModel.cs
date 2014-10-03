using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchivePumpStationViewModel : CheckBoxItemViewModel
	{
		public ArchivePumpStationViewModel(GKPumpStation pumpStation)
		{
			PumpStation = pumpStation;
			Name = pumpStation.PresentationName;
		}

		public GKPumpStation PumpStation { get; private set; }
		public string Name { get; private set; }
	}
}