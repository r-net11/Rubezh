using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OrganisationSelectionViewModel : SaveCancelDialogViewModel
	{
		public OrganisationSelectionViewModel(Organisation organisation)
		{
            Title = CommonViewModels.OrganisationSelectionViewModel_Title;
			Organisations = new ObservableCollection<OrganisationViewModel>();
			var userOrganisations = OrganisationHelper.GetByCurrentUser();
			foreach (var userOrganisation in userOrganisations)
			{
				if (userOrganisation != null)
				{
					Organisations.Add(new OrganisationViewModel(userOrganisation));
				}
			}
			if (organisation != null)
				SelectedOrganisation = Organisations.FirstOrDefault(x => x.Organisation.UID == organisation.UID);
			if (SelectedOrganisation == null)
				SelectedOrganisation = Organisations.FirstOrDefault();
		}

		public ObservableCollection<OrganisationViewModel> Organisations { get; private set; }

		OrganisationViewModel _selectedOrganisation;
		public OrganisationViewModel SelectedOrganisation
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