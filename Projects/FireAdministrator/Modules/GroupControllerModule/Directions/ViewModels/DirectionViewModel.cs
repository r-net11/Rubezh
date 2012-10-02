using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure;

namespace GKModule.ViewModels
{
    public class DirectionViewModel : BaseViewModel
    {
		public XDirection Direction { get; set; }

		public DirectionViewModel(XDirection direction)
		{
			AddZoneCommand = new RelayCommand(OnAddZone, CanAdd);
			RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemove);
			ChangeZonesCommand = new RelayCommand(OnChangeZones);

			Direction = direction;

			Zones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				if (Direction.ZoneUIDs.Contains(zone.UID))
					Zones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedZone = Zones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();

			InitializeDirectionZones();
		}

		void InitializeDirectionZones()
		{
			Zones.Clear();
			foreach (var zone in Direction.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		void InitializeDirectionDevices()
		{

		}

		public void Update()
		{
			OnPropertyChanged("Direction");
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

		public RelayCommand ChangeZonesCommand { get; private set; }
		void OnChangeZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.ZoneUIDs);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				XManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
				InitializeDirectionZones();
				ServiceFactory.SaveService.XDevicesChanged = true;
			}
		}

		public RelayCommand AddZoneCommand { get; private set; }
		void OnAddZone()
		{
			int oldIndex = SourceZones.IndexOf(SelectedSourceZone);

			Direction.ZoneUIDs.Add(SelectedSourceZone.XZone.UID);
			Zones.Add(SelectedSourceZone);
			SourceZones.Remove(SelectedSourceZone);

			if (SourceZones.Count > 0)
				SelectedSourceZone = SourceZones[System.Math.Min(oldIndex, SourceZones.Count - 1)];
		}
		bool CanAdd()
		{
			return SelectedSourceZone != null;
		}

		public RelayCommand RemoveZoneCommand { get; private set; }
		void OnRemoveZone()
		{
			int oldIndex = Zones.IndexOf(SelectedZone);

			Direction.ZoneUIDs.Remove(SelectedZone.XZone.UID);
			SourceZones.Add(SelectedZone);
			Zones.Remove(SelectedZone);

			if (Zones.Count > 0)
				SelectedZone = Zones[System.Math.Min(oldIndex, Zones.Count - 1)];
		}
		bool CanRemove()
		{
			return SelectedZone != null;
		}
	}
}