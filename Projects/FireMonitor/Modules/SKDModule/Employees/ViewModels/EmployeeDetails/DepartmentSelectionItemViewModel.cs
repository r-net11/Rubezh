using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.TreeList;
using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentSelectionItemViewModel : TreeNodeViewModel<DepartmentSelectionItemViewModel>
	{
		public ShortDepartment Department { get; private set; }

		public DepartmentSelectionItemViewModel(ShortDepartment department)
		{
			Department = department;
		}
	}
}