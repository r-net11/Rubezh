using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganizationFilter = FiresecAPI.OrganizationFilter;
using System;

namespace SKDModule.ViewModels
{
	public class NamedIntervalsViewModel : ViewPartViewModel
	{
		public NamedIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var namedIntervals = NamedIntervalHelper.Get(new NamedIntervalFilter() { OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });

			OrganisationNamedIntervals = new ObservableCollection<OrganisationNamedIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationNamedIntervalsViewModel(organisation);
				timeInrervalViewModel.Initialize(new List<NamedInterval>(namedIntervals.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationNamedIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationNamedInterval = OrganisationNamedIntervals.FirstOrDefault();
		}

		private ObservableCollection<OrganisationNamedIntervalsViewModel> _organisationNamedIntervals;
		public ObservableCollection<OrganisationNamedIntervalsViewModel> OrganisationNamedIntervals
		{
			get { return _organisationNamedIntervals; }
			set
			{
				_organisationNamedIntervals = value;
				OnPropertyChanged(() => OrganisationNamedIntervals);
			}
		}

		private OrganisationNamedIntervalsViewModel _selectedOrganisationNamedInterval;
		public OrganisationNamedIntervalsViewModel SelectedOrganisationNamedInterval
		{
			get { return _selectedOrganisationNamedInterval; }
			set
			{
				_selectedOrganisationNamedInterval = value;
				OnPropertyChanged(() => SelectedOrganisationNamedInterval);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			Initialize();
		}
	}
}