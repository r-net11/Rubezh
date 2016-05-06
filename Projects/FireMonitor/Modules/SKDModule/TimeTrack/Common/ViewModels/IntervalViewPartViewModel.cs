using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public abstract class IntervalViewPartViewModel<TOrganisationInterval, TViewModel, TElement> : ViewPartViewModel
		where TOrganisationInterval : OrganisationViewModel<TViewModel, TElement>
		where TViewModel : BaseObjectViewModel<TElement>
		where TElement : OrganisationElementBase
	{
		public IntervalViewPartViewModel()
		{
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
				timeInrervalViewModel.Initialize(models.Where(x => x.OrganisationUID == organisation.UID));
				Organisations.Add(timeInrervalViewModel);
			}
			SelectedOrganisation = Organisations.FirstOrDefault();
		}
		protected abstract IEnumerable<TElement> GetModels();
		protected abstract TOrganisationInterval CreateOrganisationViewModel(Organisation organisation);

		public IEnumerable<Organisation> GetOrganisations()
		{
			return OrganisationHelper.GetByCurrentUser();
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
	}
}