using System;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : TreeNodeViewModel<DepartmentViewModel>
	{
		public Organisation Organization { get; private set; }
		public ShortDepartment Department { get; private set; }

		public DepartmentViewModel(Organisation organisation)
		{
			Organization = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
		}

		public DepartmentViewModel(ShortDepartment department)
		{
			Department = department;
			IsOrganisation = false;
			Name = department.Name;
			Description = department.Description;
		}

		public void Update(ShortDepartment department)
		{
			Name = department.Name;
			Description = department.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public bool IsOrganisation { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }

		public void Update()
		{
			OnPropertyChanged(() => Department);
		}

		//DepartmentsViewModel DepartmentsViewModel;
		//public ShortDepartment Department { get; private set; }

		//public DepartmentViewModel(DepartmentsViewModel departmentsViewModel, ShortDepartment department)
		//{
		//    DepartmentsViewModel = departmentsViewModel;
		//    Department = department;

		//    AddCommand = new RelayCommand(OnAdd);
		//    RemoveCommand = new RelayCommand(OnRemove, CanRemove);
		//    EditCommand = new RelayCommand(OnEdit, CanEdit);
		//}

		//public void Update()
		//{
		//    OnPropertyChanged(() => Department);
		//}

		//public RelayCommand AddCommand { get; private set; }
		//void OnAdd()
		//{
		//    var departmentDetailsViewModel = new DepartmentDetailsViewModel(DepartmentsViewModel, null, Department.UID);
		//    if (DialogService.ShowModalWindow(departmentDetailsViewModel))
		//    {
		//        var department = departmentDetailsViewModel.ShortDepartment;
		//        var departmentViewModel = new DepartmentViewModel(DepartmentsViewModel, department);
		//        this.Department.ChildDepartmentUIDs.Add(department.UID);
		//        this.AddChild(departmentViewModel);
		//        this.Update();
		//        DepartmentsViewModel.Departments.Add(departmentViewModel);
		//    }
		//}

		//public RelayCommand AddToParentCommand { get; private set; }
		//void OnAddToParent()
		//{
		//    Parent.AddCommand.Execute();
		//}
		//public bool CanAddToParent()
		//{
		//    return Parent != null;
		//}

		//public RelayCommand RemoveCommand { get; private set; }
		//void OnRemove()
		//{
		//    var parent = Parent;
		//    if (parent != null)
		//    {
		//        var removeResult = DepartmentHelper.MarkDeleted(Department.UID);
		//        if (!removeResult)
		//            return;

		//        var index = ZonesViewModel.Current.SelectedZone.VisualIndex;
		//        parent.Nodes.Remove(this);
		//        parent.Update();

		//        index = Math.Min(index, parent.ChildrenCount - 1);
		//        DepartmentsViewModel.Departments.Remove(this);
		//        var children = DepartmentsViewModel.GetAllChildrenModels(this);
		//        DepartmentsViewModel.SelectedDepartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
		//    }
		//}
		//bool CanRemove()
		//{
		//    return Parent != null;
		//}

		//public RelayCommand EditCommand { get; private set; }
		//void OnEdit()
		//{
		//    var departmentDetailsViewModel = new DepartmentDetailsViewModel(DepartmentsViewModel, this.Department.UID);
		//    if (DialogService.ShowModalWindow(departmentDetailsViewModel))
		//    {
		//        this.Department = departmentDetailsViewModel.ShortDepartment;
		//        Update();
		//    }
		//}
		//public bool CanEdit()
		//{
		//    return true;
		//}
	}
}