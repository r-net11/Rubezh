using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class OrganisationZonesViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationZonesViewModel(Organisation organisation)
		{
			Organisation = organisation;

			var zones = SKDManager.SKDConfiguration.Zones;

			Zones = new ObservableCollection<OrganisationZoneViewModel>();
			foreach (var zone in zones)
			{
				Zones.Add(new OrganisationZoneViewModel(Organisation, zone));
			}
		}

		ObservableCollection<OrganisationZoneViewModel> _Zones;
		public ObservableCollection<OrganisationZoneViewModel> Zones
		{
			get { return _Zones; }
			private set
			{
				_Zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

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
