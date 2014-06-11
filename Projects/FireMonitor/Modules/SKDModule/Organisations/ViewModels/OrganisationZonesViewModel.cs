using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationZonesViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationZonesViewModel(Organisation organisation)
		{
			Organisation = organisation;

			Zones = new ObservableCollection<OrganisationZoneViewModel>();
			foreach (var zone in SKDManager.Zones)
			{
				var zoneViewModel = new OrganisationZoneViewModel(organisation, zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();

			foreach (var zone in Zones)
			{
				if (organisation.ZoneUIDs.Contains(zone.Zone.UID))
					zone._isChecked = true;
			}
		}
		public ObservableCollection<OrganisationZoneViewModel> Zones { get; private set; }

		OrganisationZoneViewModel _selectedZone;
		public OrganisationZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}
	}
}