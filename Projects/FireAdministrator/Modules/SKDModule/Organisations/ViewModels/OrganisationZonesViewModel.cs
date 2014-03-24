using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class OrganisationZonesViewModel : BaseViewModel
	{
		public Organization Organization { get; private set; }

		public OrganisationZonesViewModel(Organization organization)
		{
			Organization = organization;
			AllZones = new List<OrganisationZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = RootZone;

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
				if (organization.ZoneUIDs.Contains(zone.Zone.UID))
					zone._isChecked = true;
			}
		}

		#region Zones
		public List<OrganisationZoneViewModel> AllZones;

		OrganisationZoneViewModel _selectedZone;
		public OrganisationZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedZone");
			}
		}

		OrganisationZoneViewModel _rootZone;
		public OrganisationZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public OrganisationZoneViewModel[] RootZones
		{
			get { return new OrganisationZoneViewModel[] { RootZone }; }
		}

		OrganisationZoneViewModel AddZoneInternal(SKDZone zone, OrganisationZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new OrganisationZoneViewModel(Organization, zone);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
		#endregion
	}
}