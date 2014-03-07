using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public UserViewModel(XGuardUser guardUser)
		{
			GuardUser = guardUser;
			AddZoneCommand = new RelayCommand(OnAddZone, CanAdd);
			RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemove);

			Zones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			foreach (var zone in XManager.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				if (GuardUser.ZoneUIDs.Contains(zone.BaseUID))
					Zones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedZone = Zones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		XGuardUser _guardUser;
		public XGuardUser GuardUser
		{
			get { return _guardUser; }
			set
			{
				_guardUser = value;
				OnPropertyChanged("GuardUser");
			}
		}

		public void Update()
		{
			OnPropertyChanged("GuardUser");
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		public ObservableCollection<ZoneViewModel> SourceZones { get; private set; }

		ZoneViewModel _selectedSourceZone;
		public ZoneViewModel SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged("SelectedSourceZone");
			}
		}
		bool CanAdd()
		{
			return SelectedSourceZone != null;
		}

		public RelayCommand AddZoneCommand { get; private set; }
		void OnAddZone()
		{
			int oldIndex = SourceZones.IndexOf(SelectedSourceZone);

			GuardUser.ZoneUIDs.Add(SelectedSourceZone.Zone.BaseUID);
			Zones.Add(SelectedSourceZone);
			SourceZones.Remove(SelectedSourceZone);

			if (SourceZones.Count > 0)
				SelectedSourceZone = SourceZones[System.Math.Min(oldIndex, SourceZones.Count - 1)];

			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand RemoveZoneCommand { get; private set; }
		void OnRemoveZone()
		{
			int oldIndex = Zones.IndexOf(SelectedZone);

			GuardUser.ZoneUIDs.Remove(SelectedZone.Zone.BaseUID);
			SourceZones.Add(SelectedZone);
			Zones.Remove(SelectedZone);

			if (Zones.Count > 0)
				SelectedZone = Zones[System.Math.Min(oldIndex, Zones.Count - 1)];

			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanRemove()
		{
			return SelectedZone != null;
		}
	}
}