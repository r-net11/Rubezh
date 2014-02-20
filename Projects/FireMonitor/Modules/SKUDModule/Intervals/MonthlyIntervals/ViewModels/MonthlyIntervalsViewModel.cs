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
	public class MonthlyIntervalsViewModel : ViewPartViewModel
	{
		EmployeeMonthlyInterval IntervalToCopy;

		public MonthlyIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = FiresecManager.GetOrganizations(new OrganizationFilter());
			var employeeMonthlyIntervals = new List<EmployeeMonthlyInterval>();

			OrganisationMonthlyIntervals = new ObservableCollection<OrganisationMonthlyIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationMonthlyIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation.Name, new List<EmployeeMonthlyInterval>(employeeMonthlyIntervals.Where(x => x.OrganizationUid.Value == organisation.UID)));
				OrganisationMonthlyIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationMonthlyInterval = OrganisationMonthlyIntervals.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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
	}
}