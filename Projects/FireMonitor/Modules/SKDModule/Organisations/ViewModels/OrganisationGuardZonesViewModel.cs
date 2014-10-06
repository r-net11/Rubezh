using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganisationGuardZonesViewModel : OrganisationItemsViewModel<OrganisationGuardZoneViewModel>
	{
		public OrganisationGuardZonesViewModel(Organisation organisation)
			: base(organisation)
		{
			Items = new ObservableCollection<OrganisationGuardZoneViewModel>();
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones)
			{
				var guardZoneViewModel = new OrganisationGuardZoneViewModel(organisation, guardZone);
				Items.Add(guardZoneViewModel);
			}
			SelectedItem = Items.FirstOrDefault();
		}
	}
}