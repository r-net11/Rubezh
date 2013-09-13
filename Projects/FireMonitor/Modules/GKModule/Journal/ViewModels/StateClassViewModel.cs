using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StateClassViewModel : BaseViewModel
	{
		public StateClassViewModel(XStateClass stateClass)
		{
			StateClass = stateClass;
			Name = stateClass.ToDescription();
		}

		public XStateClass StateClass { get; private set; }
		public string Name { get; private set; }

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
