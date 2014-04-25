using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class FilterEntityViewModel : TreeNodeViewModel<FilterEntityViewModel>
	{
		public FilterEntityViewModel(Organisation organisation)
		{
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public FilterEntityViewModel(string name, string description, Guid uid)
		{
			IsOrganisation = false;
			Name = name;
			Description = description;
			UID = uid;
		}

		public bool IsOrganisation { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid UID { get; set; }

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