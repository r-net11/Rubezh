using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchivePumpStationViewModel : CheckBoxItemViewModel
	{
		public ArchivePumpStationViewModel(XPumpStation pumpStation)
		{
			PumpStation = pumpStation;
			Name = pumpStation.PresentationName;
		}

		public XPumpStation PumpStation { get; private set; }
		public string Name { get; private set; }
	}
}