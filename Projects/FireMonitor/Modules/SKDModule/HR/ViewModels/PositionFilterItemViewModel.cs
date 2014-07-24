using System;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class PositionFilterItemViewModel : TreeNodeViewModel<PositionFilterItemViewModel>
	{
		public bool IsOrganisation { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid UID { get; set; }

		public PositionFilterItemViewModel(Organisation organisation)
		{
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public PositionFilterItemViewModel(string name, string description, Guid uid)
		{
			IsOrganisation = false;
			Name = name;
			Description = description;
			UID = uid;
		}

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