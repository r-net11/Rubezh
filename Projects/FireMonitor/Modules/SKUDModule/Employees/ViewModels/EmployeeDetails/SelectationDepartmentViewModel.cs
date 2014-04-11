using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class SelectationDepartmentViewModel : TreeNodeViewModel<SelectationDepartmentViewModel>
	{
		public SelectationDepartmentViewModel(Department department)
		{
			Department = department;
		}

		public Department Department { get; private set; }
		public string Name { get { return Department.Name; } }

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
