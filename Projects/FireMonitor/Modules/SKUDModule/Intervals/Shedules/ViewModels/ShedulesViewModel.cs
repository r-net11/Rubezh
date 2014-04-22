using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganisationFilter = FiresecAPI.OrganisationFilter;

namespace SKDModule.ViewModels
{
	public class ShedulesViewModel : ViewPartViewModel
	{
		public ShedulesViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var employeeShedules = new List<Schedule>();

			OrganisationShedules = new ObservableCollection<OrganisationShedulesViewModel>();
			foreach (var organisation in organisations)
			{
				var sheduleViewModel = new OrganisationShedulesViewModel();
				sheduleViewModel.Initialize(organisation, new List<Schedule>(employeeShedules.Where(x => x.OrganisationUID.Value == organisation.UID)));
				OrganisationShedules.Add(sheduleViewModel);
			}
			SelectedOrganisationShedule = OrganisationShedules.FirstOrDefault();
		}

		ObservableCollection<OrganisationShedulesViewModel> _organisationShedule;
		public ObservableCollection<OrganisationShedulesViewModel> OrganisationShedules
		{
			get { return _organisationShedule; }
			set
			{
				_organisationShedule = value;
				OnPropertyChanged("OrganisationShedules");
			}
		}

		OrganisationShedulesViewModel _selectedOrganisationShedule;
		public OrganisationShedulesViewModel SelectedOrganisationShedule
		{
			get { return _selectedOrganisationShedule; }
			set
			{
				_selectedOrganisationShedule = value;
				OnPropertyChanged("SelectedOrganisationShedule");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}