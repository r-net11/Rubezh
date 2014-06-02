using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class PasswordGuardZonesViewModel : BaseViewModel
	{
		public Password Password { get; private set; }

		public PasswordGuardZonesViewModel(ShortPassword shortPassword)
		{
			Zones = new ObservableCollection<PasswordGuardZoneViewModel>();
			if (shortPassword != null)
			{
				Password = new Password()
				{
					UID = shortPassword.UID,
					OrganisationUID = shortPassword.OrganisationUID.Value,
					Name = shortPassword.Name,
					Description = shortPassword.Description
				};
				foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
				{
					var zoneViewModel = new PasswordGuardZoneViewModel(Password, guardZone);
					Zones.Add(zoneViewModel);
				}
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<PasswordGuardZoneViewModel> _zones;
		public ObservableCollection<PasswordGuardZoneViewModel> Zones
		{
			get { return _zones; }
			private set
			{
				_zones = value;
				OnPropertyChanged("RootZones");
			}
		}

		PasswordGuardZoneViewModel _selectedZone;
		public PasswordGuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}
	}
}