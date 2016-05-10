using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using StrazhAPI.SKD;

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