using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveDescriptionViewModel : CheckBoxItemViewModel
	{
		public ArchiveDescriptionViewModel(string description, List<string> distinctDatabaseDescriptions)
		{
			Description = description;
			IsEnabled = distinctDatabaseDescriptions.Any(x => x == description);
		}

		public string Description { get; private set; }

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		public static int Compare(ArchiveDescriptionViewModel x, ArchiveDescriptionViewModel y)
		{
			if (x.IsEnabled && !y.IsEnabled)
				return -1;
			if (!x.IsEnabled && y.IsEnabled)
				return 1;
			return x.Description.CompareTo(y.Description);
		}
	}
}