using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchivePumpStationViewModel : BaseViewModel
	{
		public ArchivePumpStationViewModel(XPumpStation pumpStation)
		{
			PumpStation = pumpStation;
		}

		public XPumpStation PumpStation { get; private set; }
		
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
