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
		where TOrganisationInterval : OrganisationViewModel<TViewModel, TElement>
		where TViewModel : BaseObjectViewModel<TElement>
		where TElement : OrganizationElementBase
	{
		public IntervalViewPartViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		protected virtual void Initialize()
		{
			var organisations = GetOrganizations();
			var models = GetModels();

			Organisations = new ObservableCollection<TOrganisationInterval>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = CreateOrganizationViewModel(organisation);
				timeInrervalViewModel.Initialize(models.Where(x => x.OrganizationUID.Value == organisation.UID));
				Organisations.Add(timeInrervalViewModel);
			}
			SelectedOrganisation = Organisations.FirstOrDefault();
		}
		protected abstract IEnumerable<TElement> GetModels();
		protected abstract TOrganisationInterval CreateOrganizationViewModel(Organisation organization);

		public IEnumerable<FiresecAPI.Organisation> GetOrganizations()
		{
			return OrganisationHelper.Get(new FiresecAPI.OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
		}

		private ObservableCollection<TOrganisationInterval> _organisations;
		public ObservableCollection<TOrganisationInterval> Organisations
		{
			get { return _organisations; }
			set
			{
				_organisations = value;
				OnPropertyChanged(() => Organisations);
			}
		}

		private TOrganisationInterval _selectedOrganisation;
		public TOrganisationInterval SelectedOrganisation
		{
			get { return _selectedOrganisation; }
			set
			{
				_selectedOrganisation = value;
				OnPropertyChanged(() => SelectedOrganisation);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			Initialize();
		}
	}
}
