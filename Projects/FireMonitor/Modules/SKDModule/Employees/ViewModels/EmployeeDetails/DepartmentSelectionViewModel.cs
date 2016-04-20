using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentSelectionViewModel : SaveCancelDialogViewModel
	{
		protected Guid _organisationUID;
		protected Guid _initialDepartmentUID;
		
		public DepartmentSelectionViewModel(Guid organisationUID, Guid departmentUID)
		{
			Title = "Выбор подразделения";
			_organisationUID = organisationUID;
			_initialDepartmentUID = departmentUID;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			ClearCommand = new RelayCommand(OnClear);
		}

		public void Initialize()
		{
			AllDepartments = new List<DepartmentSelectionItemViewModel>();
			var departments = GetDepartments();
			if (departments != null)
			{
				foreach (var department in departments)
				{
					AllDepartments.Add(new DepartmentSelectionItemViewModel(department));
				}
			}

			RootDepartments = new ObservableCollection<DepartmentSelectionItemViewModel>();
			foreach (var department in AllDepartments)
			{
				if (department.Department.ParentDepartmentUID == Guid.Empty)
				{
					RootDepartments.Add(department);
					SetChildren(department);
				}
			}

			SelectedDepartment = AllDepartments.FirstOrDefault(x => x.Department.UID == _initialDepartmentUID);
			if (SelectedDepartment != null)
			{
				SelectedDepartment.ExpandToThis();
			}
		}

		protected virtual IEnumerable<ShortDepartment> GetDepartments()
		{
			return DepartmentHelper.GetByOrganisation(_organisationUID);
		}

		void SetChildren(DepartmentSelectionItemViewModel department)
		{
			var children = AllDepartments.Where(x => department.Department.ChildDepartments.Any(y => y.UID == x.Department.UID));
			foreach (var child in children)
			{
				department.AddChild(child);
				SetChildren(child);
			}
		}

		List<DepartmentSelectionItemViewModel> AllDepartments { get; set; }
		IEnumerable<ShortDepartment> Models { get { return AllDepartments.Select(x => x.Department); } }

		public ObservableCollection<DepartmentSelectionItemViewModel> RootDepartments { get; private set; }

		DepartmentSelectionItemViewModel _selectedDepartment;
		public DepartmentSelectionItemViewModel SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			ShortDepartment parentDepartment = null;
			var hasParentDepartment = SelectedDepartment != null;
			if (hasParentDepartment)
				parentDepartment = SelectedDepartment.Department;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel();
			departmentDetailsViewModel.Initialize(_organisationUID, parentDepartment, Models);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var department = departmentDetailsViewModel.Model;
				var departmentViewModel = new DepartmentSelectionItemViewModel(department);
				if (hasParentDepartment)
				{
					SelectedDepartment.AddChild(departmentViewModel);
				}
				else
				{
					RootDepartments.Add(departmentViewModel);
					OnPropertyChanged(() => RootDepartments);
				}
				departmentViewModel.ExpandToThis();
				SelectedDepartment = departmentViewModel;
				ServiceFactory.Events.GetEvent<NewDepartmentEvent>().Publish(department);
			}
		}
		bool CanAdd()
		{
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Departments_Etit);
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			SelectedDepartment = null;
			Close();
		}

		protected override bool Save()
		{
			return true;
		}
	}

	public class DepartmentParentSelectionViewModel : DepartmentSelectionViewModel
	{
		Guid _departmentUID;

		public DepartmentParentSelectionViewModel(Guid organisationUID, Guid parentDepartmentUID, Guid departmentUID) : base(organisationUID, parentDepartmentUID) 
		{
			_departmentUID = departmentUID;
			Title = "Выбор родительского подразделения";
		}
 
		protected override IEnumerable<ShortDepartment> GetDepartments()
		{
			var filter = new DepartmentFilter { OrganisationUIDs = new List<Guid> { _organisationUID }, ExceptUIDs = new List<Guid> { _departmentUID } };
			return DepartmentHelper.Get(filter);
		}
	}
}