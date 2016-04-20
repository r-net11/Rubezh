using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public Direction Direction { get; private set; }

		public DirectionViewModel(Direction direction)
		{
			AddZoneCommand = new RelayCommand(OnAddZone, CanAdd);
			RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemove);

			Direction = direction;

			Zones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			foreach (var zone in FiresecManager.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				if (Direction.ZoneUIDs.Contains(zone.UID))
					Zones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedZone = Zones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public void Update()
		{
			OnPropertyChanged(() => Direction);
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
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
				OnPropertyChanged(() => SelectedSourceZone);
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

			Direction.ZoneUIDs.Add(SelectedSourceZone.Zone.UID);
			Zones.Add(SelectedSourceZone);
			SourceZones.Remove(SelectedSourceZone);

			if (SourceZones.Count > 0)
				SelectedSourceZone = SourceZones[System.Math.Min(oldIndex, SourceZones.Count - 1)];
		}

		bool CanRemove()
		{
			return SelectedZone != null;
		}

		public RelayCommand RemoveZoneCommand { get; private set; }
		void OnRemoveZone()
		{
			int oldIndex = Zones.IndexOf(SelectedZone);

			Direction.ZoneUIDs.Remove(SelectedZone.Zone.UID);
			SourceZones.Add(SelectedZone);
			Zones.Remove(SelectedZone);

			if (Zones.Count > 0)
				SelectedZone = Zones[System.Math.Min(oldIndex, Zones.Count - 1)];
		}
	}
}