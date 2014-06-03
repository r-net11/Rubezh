using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationGuardZonesViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationGuardZonesViewModel(Organisation organisation)
		{
			Organisation = organisation;
			GuardZones = new ObservableCollection<OrganisationGuardZoneViewModel>();
			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
			{
				var guardZoneViewModel = new OrganisationGuardZoneViewModel(organisation, guardZone);
				GuardZones.Add(guardZoneViewModel);
			}
			SelectedGuardZone = GuardZones.FirstOrDefault();
		}

		ObservableCollection<OrganisationGuardZoneViewModel> _guardZones;
		public ObservableCollection<OrganisationGuardZoneViewModel> GuardZones
		{
			get { return _guardZones; }
			private set
			{
				_guardZones = value;
				OnPropertyChanged("GuardZones");
			}
		}

		OrganisationGuardZoneViewModel _selectedGuardZone;
		public OrganisationGuardZoneViewModel SelectedGuardZone
		{
			get { return _selectedGuardZone; }
			set
			{
				_selectedGuardZone = value;
				OnPropertyChanged("SelectedGuardZone");
			}
		}
	}
}