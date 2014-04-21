using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : ViewPartViewModel
	{
		DepartmentFilter Filter;

		public DepartmentsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new DepartmentFilter();
			Initialize();
		}

		void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			OrganisationDepartments = new ObservableCollection<OrganisationDepartmentsViewModel>();
			foreach (var organisation in organisations)
			{
				var departmentViewModel = new OrganisationDepartmentsViewModel();
				var departments = DepartmentHelper.GetByOrganization(organisation.UID);
				departmentViewModel.Initialize(organisation, departments);
				OrganisationDepartments.Add(departmentViewModel);
			}
			SelectedOrganisationDepartment = OrganisationDepartments.FirstOrDefault();
		}

		ObservableCollection<OrganisationDepartmentsViewModel> _organisationDepartments;
		public ObservableCollection<OrganisationDepartmentsViewModel> OrganisationDepartments
		{
			get { return _organisationDepartments; }
			set
			{
				_organisationDepartments = value;
				OnPropertyChanged("OrganisationDepartments");
			}
		}

		OrganisationDepartmentsViewModel _selectedOrganisationDepartment;
		public OrganisationDepartmentsViewModel SelectedOrganisationDepartment
		{
			get { return _selectedOrganisationDepartment; }
			set
			{
				_selectedOrganisationDepartment = value;
				OnPropertyChanged("SelectedOrganisationDepartment");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var departmentFilterViewModel = new DepartmentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(departmentFilterViewModel))
			{
				Filter = departmentFilterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}