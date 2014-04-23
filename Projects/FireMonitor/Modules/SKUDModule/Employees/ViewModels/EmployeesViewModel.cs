using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public static EmployeesViewModel Current { get; private set; }
		public PersonType PersonType { get; private set; }
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }

		public EmployeesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(EmployeeFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employees = EmployeeHelper.Get(filter);

			AllEmployees = new List<EmployeeViewModel>();
			Organisations = new List<EmployeeViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new EmployeeViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllEmployees.Add(organisationViewModel);
				foreach (var employee in employees)
				{
					if (employee.OrganisationUID == organisation.UID)
					{
						var employeeViewModel = new EmployeeViewModel(employee);
						organisationViewModel.AddChild(employeeViewModel);
						AllEmployees.Add(employeeViewModel);
					}
				}
			}

			foreach (var organisation in Organisations)
			{
				organisation.ExpandToThis();
			}
			OnPropertyChanged("RootEmployees");
		}

		List<EmployeeViewModel> _organisations;
		public List<EmployeeViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public Organisation Organisation
		{
			get
			{
				EmployeeViewModel OrganisationViewModel = SelectedEmployee;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedEmployee.Parent;

				if (OrganisationViewModel != null)
					return OrganisationViewModel.Organisation;

				return null;
			}
		}

		public EmployeeViewModel[] RootEmployees
		{
			get { return Organisations.ToArray(); }
		}

		#region EmployeeSelection
		public List<EmployeeViewModel> AllEmployees;

		public void Select(Guid employeeUID)
		{
			if (employeeUID != Guid.Empty)
			{
				var employeeViewModel = AllEmployees.FirstOrDefault(x => x.ShortEmployee != null && x.ShortEmployee.UID == employeeUID);
				if (employeeViewModel != null)
					employeeViewModel.ExpandToThis();
				SelectedEmployee = employeeViewModel;
			}
		}
		#endregion

		EmployeeViewModel _selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employeeViewModel = new EmployeeViewModel(employeeDetailsViewModel.ShortEmployee);
				AllEmployees.Add(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this, SelectedEmployee.ShortEmployee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.ShortEmployee);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var employee = SelectedEmployee.ShortEmployee;
			var removeResult = EmployeeHelper.MarkDeleted(employee.UID);
			if (!removeResult)
				return;

			var index = AllEmployees.IndexOf(SelectedEmployee);
			AllEmployees.Remove(SelectedEmployee);
			index = Math.Min(index, AllEmployees.Count - 1);
			if (index > -1)
				SelectedEmployee = AllEmployees[index];
		}
		bool CanRemove()
		{
			return SelectedEmployee != null;
		}

		public void InitializeAdditionalColumns()
		{
			//AdditionalColumnNames = new ObservableCollection<string>();
			//var additionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			//if (Organisation != null)
			//    additionalColumnTypeFilter.OrganisationUIDs.Add(Organisation.UID);
			//var columnTypes = AdditionalColumnTypeHelper.Get(additionalColumnTypeFilter);
			//if (columnTypes == null)
			//    return;
			//AdditionalColumnTypes = columnTypes.ToList();
			//foreach (var additionalColumnType in AdditionalColumnTypes)
			//{
			//    if (additionalColumnType.DataType == AdditionalColumnDataType.Text && additionalColumnType.IsInGrid)
			//        AdditionalColumnNames.Add(additionalColumnType.Name);
			//}
			//foreach (var employee in AllEmployees)
			//{
			//    employee.AdditionalColumnValues = new ObservableCollection<string>();
			//    foreach (var additionalColumnType in AdditionalColumnTypes)
			//    {
			//        if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
			//            employee.AdditionalColumnValues.Add("Test " + additionalColumnType.Name);
			//    }
			//}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}