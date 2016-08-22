using System.Collections.ObjectModel;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : OrganisationItemsViewModel<OrganisationDoorViewModel>
	{
		public OrganisationDoorsViewModel(Organisation organisation)
			: base(organisation)
		{
			Items = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Doors; }
		}
	}
}