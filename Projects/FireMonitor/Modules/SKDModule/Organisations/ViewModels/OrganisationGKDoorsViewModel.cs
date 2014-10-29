using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganisationGKDoorsViewModel : OrganisationItemsViewModel<OrganisationGKDoorViewModel>
	{
		public OrganisationGKDoorsViewModel(Organisation organisation):base(organisation)
		{
			Items = new ObservableCollection<OrganisationGKDoorViewModel>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				Items.Add(new OrganisationGKDoorViewModel(Organisation, door));
			}
		}
	}
}