using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateGuardZonesViewModel : BaseViewModel
	{
		public AccessTemplate AccessTemplate { get; private set; }
		Organisation Organisation;

		public AccessTemplateGuardZonesViewModel(AccessTemplate accessTemplate, Organisation organisation)
		{
			AccessTemplate = accessTemplate;
			Organisation = organisation;
			GuardZones = new ObservableCollection<AccessTemplateGuardZoneViewModel>();
			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
			{
				if (Organisation.GuardZoneUIDs.Any(x => x == guardZone.UID))
				{
					var guardZoneViewModel = new AccessTemplateGuardZoneViewModel(accessTemplate, guardZone);
					GuardZones.Add(guardZoneViewModel);
				}
			}
			SelectedGuardZone = GuardZones.FirstOrDefault();
			HasZones = GuardZones.Count > 0;
		}

		ObservableCollection<AccessTemplateGuardZoneViewModel> _guardZones;
		public ObservableCollection<AccessTemplateGuardZoneViewModel> GuardZones
		{
			get { return _guardZones; }
			private set
			{
				_guardZones = value;
				OnPropertyChanged(() => GuardZones);
			}
		}

		AccessTemplateGuardZoneViewModel _selectedGuardZone;
		public AccessTemplateGuardZoneViewModel SelectedGuardZone
		{
			get { return _selectedGuardZone; }
			set
			{
				_selectedGuardZone = value;
				OnPropertyChanged(() => SelectedGuardZone);
			}
		}

		public bool HasZones { get; private set; }
	}
}