using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentSelectionViewModel : SaveCancelDialogViewModel
	{
		protected Guid _organisationUID;
		protected Guid _firstSelectedDepartmentUID;

		private bool _showApplyToEmployeeSettings;
		/// <summary>
		/// Показывать или нет настройки, применяемые для пользователя
		/// </summary>
		public bool ShowApplyToEmployeeSettings
		{
			get { return _showApplyToEmployeeSettings; }
			set
			{
				_showApplyToEmployeeSettings = value;
				OnPropertyChanged(() => ShowApplyToEmployeeSettings);
			}
		}

		private bool _isEmployee;
		/// <summary>
		/// Для кого (сотрудник или посетитель) было вызвано данное окно:
		/// true, если окно было вызвано для сотрудника
		/// false - в противном случае
		/// </summary>
		public bool IsEmployee
		{
			get { return _isEmployee; }
			set
			{
				_isEmployee = value;
				OnPropertyChanged(() => IsEmployee);
			}
		}

		private bool _needApplyAccessTemplateToEmployee;
		/// <summary>
		/// Состояния чекбокса "Применить шаблоно доступа к сотруднику/посетителю"
		/// </summary>
		public bool NeedApplyAccessTemplateToEmployee
		{
			get { return _needApplyAccessTemplateToEmployee; }
			set
			{
				_needApplyAccessTemplateToEmployee = value;
				OnPropertyChanged(() => NeedApplyAccessTemplateToEmployee);
			}
		}

		private bool _needApplyScheduleToEmployee;
		/// <summary>
		/// Состояния чекбокса "Применить график работы к сотруднику"
		/// </summary>
		public bool NeedApplyScheduleToEmployee
		{
			get { return _needApplyScheduleToEmployee; }
			set
			{
				_needApplyScheduleToEmployee = value;
				OnPropertyChanged(() => NeedApplyScheduleToEmployee);
			}
		}

		public DepartmentSelectionViewModel(Guid organisationUID, Guid departmentUID)
		{
			Title = "Выбор подразделения";
			_organisationUID = organisationUID;
			_firstSelectedDepartmentUID = departmentUID;
			AddCommand = new RelayCommand(OnAdd, () => FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Departments_Etit));
			ClearCommand = new RelayCommand(OnClear, () => SelectedDepartment != null);
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
				if (department.Department.ParentDepartmentUID.IsNullOrEmpty()) //проверка на null добавлена для совместимости с предыдущими версиями. В новых версиях ParentDepartmentUID никогда не может является null.
				{
					RootDepartments.Add(department);
					SetChildren(department);
				}
			}

			SelectedDepartment = AllDepartments.FirstOrDefault(x => x.Department.UID == _firstSelectedDepartmentUID);
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
			var children = AllDepartments.Where(x => department.Department.ChildDepartments.Any(y => y.Key == x.Department.UID));
			foreach (var child in children)
			{
				department.AddChild(child);
				SetChildren(child);
			}
		}

		List<DepartmentSelectionItemViewModel> AllDepartments { get; set; }

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
			Guid? parentDepartmentUID = null;
			var hasParentDepartment = SelectedDepartment != null;
			if (hasParentDepartment)
				parentDepartmentUID = SelectedDepartment.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel();
			departmentDetailsViewModel.Initialize(_organisationUID, parentDepartmentUID);

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
				ServiceFactoryBase.Events.GetEvent<NewDepartmentEvent>().Publish(department);
			}
		}

		public RelayCommand ClearCommand { get; private set; }

		void OnClear()
		{
			SelectedDepartment = null;
		}

		protected override bool Save()
		{
			return true;
		}
	}

	public class DepartmentParentSelectionViewModel : DepartmentSelectionViewModel
	{
		readonly Guid _departmentUID;

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