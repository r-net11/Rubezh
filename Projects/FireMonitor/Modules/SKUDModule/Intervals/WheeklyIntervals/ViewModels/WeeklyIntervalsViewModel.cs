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
	public class WeeklyIntervalsViewModel : ViewPartViewModel
	{
		public WeeklyIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeWeeklyIntervals = new List<ScheduleScheme>();

			OrganisationWeeklyIntervals = new ObservableCollection<OrganisationWeeklyIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationWeeklyIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation, new List<ScheduleScheme>(employeeWeeklyIntervals.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationWeeklyIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationWeeklyInterval = OrganisationWeeklyIntervals.FirstOrDefault();
		}

		ObservableCollection<OrganisationWeeklyIntervalsViewModel> _organisationWeeklyIntervals;
		public ObservableCollection<OrganisationWeeklyIntervalsViewModel> OrganisationWeeklyIntervals
		{
			get { return _organisationWeeklyIntervals; }
			set
			{
				_organisationWeeklyIntervals = value;
				OnPropertyChanged("OrganisationWeeklyIntervals");
			}
		}

		OrganisationWeeklyIntervalsViewModel _selectedOrganisationWeeklyInterval;
		public OrganisationWeeklyIntervalsViewModel SelectedOrganisationWeeklyInterval
		{
			get { return _selectedOrganisationWeeklyInterval; }
			set
			{
				_selectedOrganisationWeeklyInterval = value;
				OnPropertyChanged("SelectedOrganisationWeeklyInterval");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}