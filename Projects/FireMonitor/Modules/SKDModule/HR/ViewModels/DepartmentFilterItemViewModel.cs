using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DepartmentFilterItemViewModel : TreeNodeViewModel<DepartmentFilterItemViewModel>
	{
		public bool IsOrganisation { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid UID { get; set; }
		public ShortDepartment Department { get; private set; }

		public DepartmentFilterItemViewModel(Organisation organisation)
		{
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public DepartmentFilterItemViewModel(ShortDepartment department)
		{
			Department = department;
			Name = department.Name;
			Description = department.Description;
			UID = department.UID;
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