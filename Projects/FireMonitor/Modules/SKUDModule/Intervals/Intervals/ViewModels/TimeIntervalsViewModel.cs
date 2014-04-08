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
	public class TimeIntervalsViewModel : ViewPartViewModel
	{
		public TimeIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeTimeIntervals = new List<NamedInterval>();

			OrganisationTimeIntervals = new ObservableCollection<OrganisationTimeIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationTimeIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation, new List<NamedInterval>(employeeTimeIntervals.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationTimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationTimeInterval = OrganisationTimeIntervals.FirstOrDefault();
		}

		ObservableCollection<OrganisationTimeIntervalsViewModel> _organisationTimeIntervals;
		public ObservableCollection<OrganisationTimeIntervalsViewModel> OrganisationTimeIntervals
		{
			get { return _organisationTimeIntervals; }
			set
			{
				_organisationTimeIntervals = value;
				OnPropertyChanged("OrganisationTimeIntervals");
			}
		}

		OrganisationTimeIntervalsViewModel _selectedOrganisationTimeInterval;
		public OrganisationTimeIntervalsViewModel SelectedOrganisationTimeInterval
		{
			get { return _selectedOrganisationTimeInterval; }
			set
			{
				_selectedOrganisationTimeInterval = value;
				OnPropertyChanged("SelectedOrganisationTimeInterval");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}