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
	public class SlideDayIntervalsViewModel : ViewPartViewModel
	{
		public SlideDayIntervalsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeSlideDayIntervals = new List<EmployeeSlideDayInterval>();

			OrganisationSlideDayIntervals = new ObservableCollection<OrganisationSlideDayIntervalsViewModel>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = new OrganisationSlideDayIntervalsViewModel();
				timeInrervalViewModel.Initialize(organisation, new List<EmployeeSlideDayInterval>(employeeSlideDayIntervals.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationSlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationSlideDayInterval = OrganisationSlideDayIntervals.FirstOrDefault();
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

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}