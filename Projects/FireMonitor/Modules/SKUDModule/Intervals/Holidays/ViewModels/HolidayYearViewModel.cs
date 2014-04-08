using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using OrganizationFilter = FiresecAPI.OrganizationFilter;

namespace SKDModule.ViewModels
{
	public class HolidayYearViewModel : BaseViewModel
	{
		public int Year { get; private set; }

		public HolidayYearViewModel(int year)
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Year = year;
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeHolidays = new List<Holiday>();

			OrganisationHolidays = new ObservableCollection<OrganisationHolidaysYearViewModel>();
			foreach (var organisation in organisations)
			{
				var holidayViewModel = new OrganisationHolidaysYearViewModel(Year);
				holidayViewModel.Initialize(organisation, new List<Holiday>(employeeHolidays.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationHolidays.Add(holidayViewModel);
			}
			SelectedOrganisationHoliday = OrganisationHolidays.FirstOrDefault();
		}

		ObservableCollection<OrganisationHolidaysYearViewModel> _organisationHolidays;
		public ObservableCollection<OrganisationHolidaysYearViewModel> OrganisationHolidays
		{
			get { return _organisationHolidays; }
			set
			{
				_organisationHolidays = value;
				OnPropertyChanged("OrganisationHolidays");
			}
		}

		OrganisationHolidaysYearViewModel _selectedOrganisationHoliday;
		public OrganisationHolidaysYearViewModel SelectedOrganisationHoliday
		{
			get { return _selectedOrganisationHoliday; }
			set
			{
				_selectedOrganisationHoliday = value;
				OnPropertyChanged("SelectedOrganisationHoliday");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}