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
		where TElement : OrganisationElementBase
	{
		public IntervalViewPartViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		protected virtual void Initialize()
		{
			var organisations = GetOrganisations();
			var models = GetModels();

			Organisations = new ObservableCollection<TOrganisationInterval>();
			foreach (var organisation in organisations)
			{
				var timeInrervalViewModel = CreateOrganisationViewModel(organisation);
				timeInrervalViewModel.Initialize(models.Where(x => x.OrganisationUID.Value == organisation.UID));
				Organisations.Add(timeInrervalViewModel);
			}
			SelectedOrganisation = Organisations.FirstOrDefault();
		}
		protected abstract IEnumerable<TElement> GetModels();
		protected abstract TOrganisationInterval CreateOrganisationViewModel(Organisation organisation);

		public IEnumerable<FiresecAPI.Organisation> GetOrganisations()
		{
			return OrganisationHelper.Get(new FiresecAPI.OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
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