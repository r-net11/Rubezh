using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using FiresecClient;

namespace SKDModule.Intervals.Common.ViewModels
{
	public abstract class IntervalViewPartViewModel<TOrganisationInterval, TViewModel, TElement> : ViewPartViewModel
		where TOrganisationInterval : OrganisationIntervalViewModel<TViewModel, TElement>
		where TViewModel : BaseObjectViewModel<TElement>
		where TElement : OrganizationElementBase
	{
		public IntervalViewPartViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public virtual void Initialize()
		{
			var organisations = GetOrganizations();
			var models = GetModels();

			OrganisationIntervals = new ObservableCollection<TOrganisationInterval>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = CreateOrganizationViewModel(organisation);
				timeInrervalViewModel.Initialize(models.Where(x => x.OrganizationUID.Value == organisation.UID));
				OrganisationIntervals.Add(timeInrervalViewModel);
			}
			SelectedOrganisationInterval = OrganisationIntervals.FirstOrDefault();
		}
		public abstract IEnumerable<TElement> GetModels();
		public abstract TOrganisationInterval CreateOrganizationViewModel(Organisation organization);

		public IEnumerable<FiresecAPI.Organisation> GetOrganizations()
		{
			return OrganisationHelper.Get(new FiresecAPI.OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
		}

		private ObservableCollection<TOrganisationInterval> _organisationIntervals;
		public ObservableCollection<TOrganisationInterval> OrganisationIntervals
		{
			get { return _organisationIntervals; }
			set
			{
				_organisationIntervals = value;
				OnPropertyChanged(() => OrganisationIntervals);
			}
		}

		private TOrganisationInterval _selectedOrganisationInterval;
		public TOrganisationInterval SelectedOrganisationInterval
		{
			get { return _selectedOrganisationInterval; }
			set
			{
				_selectedOrganisationInterval = value;
				OnPropertyChanged(() => SelectedOrganisationInterval);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			Initialize();
		}
	}
}
