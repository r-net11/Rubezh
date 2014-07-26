using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModelLastNameComparer : TreeNodeComparer<EmployeeViewModel>
	{
		protected override int Compare(EmployeeViewModel x, EmployeeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PositionViewModelNameComparer : TreeNodeComparer<PositionViewModel>
	{
		protected override int Compare(PositionViewModel x, PositionViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PositionViewModelDescriptionComparer : TreeNodeComparer<PositionViewModel>
	{
		protected override int Compare(PositionViewModel x, PositionViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class DepartmentViewModelNameComparer : TreeNodeComparer<DepartmentViewModel>
	{
		protected override int Compare(DepartmentViewModel x, DepartmentViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class DepartmentViewModelDescriptionComparer : TreeNodeComparer<DepartmentViewModel>
	{
		protected override int Compare(DepartmentViewModel x, DepartmentViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class AdditionalColumnTypeViewModelNameComparer : TreeNodeComparer<AdditionalColumnTypeViewModel>
	{
		protected override int Compare(AdditionalColumnTypeViewModel x, AdditionalColumnTypeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class AdditionalColumnTypeViewModelDescriptionComparer : TreeNodeComparer<AdditionalColumnTypeViewModel>
	{
		protected override int Compare(AdditionalColumnTypeViewModel x, AdditionalColumnTypeViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}