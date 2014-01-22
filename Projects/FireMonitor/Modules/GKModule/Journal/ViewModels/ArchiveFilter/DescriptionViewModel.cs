using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
	public class DescriptionViewModel : BaseViewModel
	{
		public DescriptionViewModel(Description description, List<string> distinctDatabaseDescriptions)
		{
			Description = description;
            IsEnabled = distinctDatabaseDescriptions.Any(x => x == description.Name);
		}

		public Description Description { get; private set; }

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

        public static int Compare(DescriptionViewModel x, DescriptionViewModel y)
        {
            if (x.IsEnabled && !y.IsEnabled)
                return -1;
            if (!x.IsEnabled && y.IsEnabled)
                return 1;
            return 0;
        }
	}
}
