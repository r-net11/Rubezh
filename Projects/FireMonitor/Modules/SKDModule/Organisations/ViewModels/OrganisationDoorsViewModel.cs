using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : OrganisationItemsViewModel<OrganisationDoorViewModel>
	{
		public OrganisationDoorsViewModel(Organisation organisation, bool isConnected):base(organisation, isConnected)
		{
			Items = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)  
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Doors; }
		}
	}
}