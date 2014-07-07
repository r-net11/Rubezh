using System.Collections.ObjectModel;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class OrganisationZonesViewModel : OrganisationItemsViewModel<OrganisationZoneViewModel>
	{
		public OrganisationZonesViewModel(Organisation organisation):base(organisation)
		{
			var zones = SKDManager.SKDConfiguration.Zones;
			Items = new ObservableCollection<OrganisationZoneViewModel>();
			foreach (var zone in zones)
			{
				Items.Add(new OrganisationZoneViewModel(Organisation, zone));
			}
		}
	}
}
