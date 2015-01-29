using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OrganisationSelectionViewModel : SaveCancelDialogViewModel
	{
		public OrganisationSelectionViewModel(Organisation organisation)
		{
			Title = "Выбор организации";
			Organisations = new ObservableCollection<OrganisationViewModel>();
			OrganisationHelper.GetByCurrentUser().ToList().ForEach(x => Organisations.Add(new OrganisationViewModel(x)));
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
