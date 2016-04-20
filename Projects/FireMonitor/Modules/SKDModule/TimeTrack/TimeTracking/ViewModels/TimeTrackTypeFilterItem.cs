using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class TimeTrackTypeFilterItem : BaseViewModel
	{
		public TimeTrackTypeFilterItem(TimeTrackType timeTrackType)
		{
			TimeTrackType = timeTrackType;
		}

		public TimeTrackType TimeTrackType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}