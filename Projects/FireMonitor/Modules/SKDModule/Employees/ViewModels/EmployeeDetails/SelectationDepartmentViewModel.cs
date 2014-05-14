using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class SelectationDepartmentViewModel : TreeNodeViewModel<SelectationDepartmentViewModel>
	{
		SelectDepartmentViewModel SelectDepartmentViewModel;
		
		public SelectationDepartmentViewModel(ShortDepartment department, SelectDepartmentViewModel selectDepartmentViewModel)
		{
			Department = department;
			SelectDepartmentViewModel = selectDepartmentViewModel;
			SelectCommand = new RelayCommand(OnSelect);
		}

		public ShortDepartment Department { get; private set; }
		public string Name { get { return Department.Name; } }
		public string Description { get { return Department.Description; } }

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

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			var department = SelectDepartmentViewModel.Departments.FirstOrDefault(x => x.IsChecked);
			if (department != null)
				department.IsChecked = false;
			IsChecked = true;
			SelectDepartmentViewModel.HighlightedDepartment = this;
		}
	}
}
