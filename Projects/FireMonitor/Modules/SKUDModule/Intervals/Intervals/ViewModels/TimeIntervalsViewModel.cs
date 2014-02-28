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
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class TimeIntervalsViewModel : ViewPartViewModel
	{
		EmployeeTimeInterval IntervalToCopy;

		public TimeIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var employeeTimeIntervals = new List<EmployeeTimeInterval>();

			OrganisationTimeIntervals = new ObservableCollection<OrganisationTimeIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationTimeIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation.Name, new List<EmployeeTimeInterval>(employeeTimeIntervals.Where(x => x.OrganizationUid.Value == organisation.UID)));
				OrganisationTimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationTimeInterval = OrganisationTimeIntervals.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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
	}
}