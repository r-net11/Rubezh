using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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