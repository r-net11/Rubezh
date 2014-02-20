using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalsViewModel : ViewPartViewModel
	{
		EmployeeWeeklyInterval IntervalToCopy;

		public WeeklyIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = FiresecManager.GetOrganizations(new OrganizationFilter());
			var employeeWeeklyIntervals = new List<EmployeeWeeklyInterval>();

			OrganisationWeeklyIntervals = new ObservableCollection<OrganisationWeeklyIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationWeeklyIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation.Name, new List<EmployeeWeeklyInterval>(employeeWeeklyIntervals.Where(x => x.OrganizationUid.Value == organisation.UID)));
				OrganisationWeeklyIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationWeeklyInterval = OrganisationWeeklyIntervals.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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
	}
}