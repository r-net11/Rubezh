using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;
using FiresecClient.SKDHelpers;
using OrganizationFilter = FiresecAPI.OrganizationFilter;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalsViewModel : ViewPartViewModel
	{
		public MonthlyIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeMonthlyIntervals = new List<ScheduleScheme>();

			OrganisationMonthlyIntervals = new ObservableCollection<OrganisationMonthlyIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationMonthlyIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation, new List<ScheduleScheme>(employeeMonthlyIntervals.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationMonthlyIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationMonthlyInterval = OrganisationMonthlyIntervals.FirstOrDefault();
		}

		ObservableCollection<OrganisationMonthlyIntervalsViewModel> _organisationMonthlyIntervals;
		public ObservableCollection<OrganisationMonthlyIntervalsViewModel> OrganisationMonthlyIntervals
		{
			get { return _organisationMonthlyIntervals; }
			set
			{
				_organisationMonthlyIntervals = value;
				OnPropertyChanged("OrganisationMonthlyIntervals");
			}
		}

		OrganisationMonthlyIntervalsViewModel _selectedOrganisationMonthlyInterval;
		public OrganisationMonthlyIntervalsViewModel SelectedOrganisationMonthlyInterval
		{
			get { return _selectedOrganisationMonthlyInterval; }
			set
			{
				_selectedOrganisationMonthlyInterval = value;
				OnPropertyChanged("SelectedOrganisationMonthlyInterval");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}