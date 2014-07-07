using System.Collections.ObjectModel;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : OrganisationItemsViewModel<OrganisationDoorViewModel>
	{
		public OrganisationDoorsViewModel(Organisation organisation):base(organisation)
		{
			var doors = SKDManager.SKDConfiguration.Doors;

			Items = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in doors)
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}
	}
}