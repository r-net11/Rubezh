using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class FilterDepartmentViewModel : FilterEntityViewModel
	{
		public ShortDepartment Department { get; private set; }

		public FilterDepartmentViewModel(ShortDepartment department)
			: base(department.Name, department.Description, department.UID)
		{
			Department = department;
		}

		public FilterDepartmentViewModel(Organisation organisation) : base(organisation) { }
	}
}