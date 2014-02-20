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
	public class SlideDayIntervalsViewModel : ViewPartViewModel
	{
		EmployeeSlideDayInterval IntervalToCopy;

		public SlideDayIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = FiresecManager.GetOrganizations(new OrganizationFilter());
			var employeeSlideDayIntervals = new List<EmployeeSlideDayInterval>();

			OrganisationSlideDayIntervals = new ObservableCollection<OrganisationSlideDayIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationSlideDayIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation.Name, new List<EmployeeSlideDayInterval>(employeeSlideDayIntervals.Where(x => x.OrganizationUid.Value == organisation.UID)));
				OrganisationSlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationSlideDayInterval = OrganisationSlideDayIntervals.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<OrganisationSlideDayIntervalsViewModel> _organisationSlideDayIntervals;
		public ObservableCollection<OrganisationSlideDayIntervalsViewModel> OrganisationSlideDayIntervals
		{
			get { return _organisationSlideDayIntervals; }
			set
			{
				_organisationSlideDayIntervals = value;
				OnPropertyChanged("OrganisationSlideDayIntervals");
			}
		}

		OrganisationSlideDayIntervalsViewModel _selectedOrganisationSlideDayInterval;
		public OrganisationSlideDayIntervalsViewModel SelectedOrganisationSlideDayInterval
		{
			get { return _selectedOrganisationSlideDayInterval; }
			set
			{
				_selectedOrganisationSlideDayInterval = value;
				OnPropertyChanged("SelectedOrganisationSlideDayInterval");
			}
		}
	}
}