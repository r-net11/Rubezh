using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : OrganisationItemsViewModel<OrganisationDoorViewModel>
	{
		public OrganisationDoorsViewModel(Organisation organisation):base(organisation)
		{
			Items = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}
	}
}