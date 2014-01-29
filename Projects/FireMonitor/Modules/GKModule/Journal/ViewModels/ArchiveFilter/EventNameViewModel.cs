using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
	public class EventNameViewModel : CheckBoxItemViewModel
	{
		public EventNameViewModel(EventName eventName, List<string> distinctDatabaseDescriptions)
		{
			EventName = eventName;
			IsEnabled = distinctDatabaseDescriptions.Any(x => x == eventName.Name);
		}

		public EventName EventName { get; private set; }
		
		bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

		public static int Compare(EventNameViewModel x, EventNameViewModel y)
		{
			if (x.IsEnabled && !y.IsEnabled)
				return -1;
			if (!x.IsEnabled && y.IsEnabled)
				return 1;
			return x.EventName.Name.CompareTo(y.EventName.Name);
		}
	}
}