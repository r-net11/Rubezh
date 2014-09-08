using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class OrganisationCardTemplatesViewModel : OrganisationItemsViewModel<OrganisationCardTemplateViewModel>
	{
		public OrganisationCardTemplatesViewModel(Organisation organisation)
			: base(organisation)
		{
			Items = new ObservableCollection<OrganisationCardTemplateViewModel>();
			//foreach (var cardTemplate in SKDManager.SKDPassCardLibraryConfiguration.Templates)
			//{
			//    var CardTemplateViewModel = new OrganisationCardTemplateViewModel(organisation, cardTemplate);
			//    Items.Add(CardTemplateViewModel);
			//}
			SelectedItem = Items.FirstOrDefault();
		}
	}
}
