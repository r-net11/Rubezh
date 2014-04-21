using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
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
			var holidays = HolidayHelper.Get(new HolidayFilter() 
			{ 
				OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs,
				Year = Year,
			});

			OrganisationHolidays = new ObservableCollection<OrganisationHolidaysYearViewModel>();
			foreach (var organisation in organisations)
			{
				var holidayViewModel = new OrganisationHolidaysYearViewModel(Year, organisation);
				holidayViewModel.Initialize(new List<Holiday>(holidays.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationHolidays.Add(holidayViewModel);
			}
			SelectedOrganisationHoliday = OrganisationHolidays.FirstOrDefault();
		}

		private ObservableCollection<OrganisationHolidaysYearViewModel> _organisationHolidays;
		public ObservableCollection<OrganisationHolidaysYearViewModel> OrganisationHolidays
		{
			get { return _organisationHolidays; }
			set
			{
				_organisationHolidays = value;
				OnPropertyChanged(() => OrganisationHolidays);
			}
		}

		private OrganisationHolidaysYearViewModel _selectedOrganisationHoliday;
		public OrganisationHolidaysYearViewModel SelectedOrganisationHoliday
		{
			get { return _selectedOrganisationHoliday; }
			set
			{
				_selectedOrganisationHoliday = value;
				OnPropertyChanged(() => SelectedOrganisationHoliday);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}