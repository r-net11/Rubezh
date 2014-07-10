using System.Linq;
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
			Name = Department.Name;
			Description = Department.Description;
			SelectDepartmentViewModel = selectDepartmentViewModel;
			SelectCommand = new RelayCommand(OnSelect);
			IsDepartment = true;
		}

		public SelectationDepartmentViewModel(Organisation organisation, SelectDepartmentViewModel selectDepartmentViewModel)
		{
			Organisation = organisation;
			Name = organisation.Name;
			Description = organisation.Description;
			SelectDepartmentViewModel = selectDepartmentViewModel;
			SelectCommand = new RelayCommand(OnSelect);
			IsDepartment = false;
		}

		public ShortDepartment Department { get; private set; }
		public Organisation Organisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsDepartment { get; private set; } 

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