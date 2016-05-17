using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class ScheduleZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public List <ScheduleZone> ScheduleZone { get; private set; }
		List<Guid> _doorUIDs;

		public ScheduleZoneDetailsViewModel(Schedule schedule, Organisation organisation)
		{
			Title = "Добавить новые зоны";
			ScheduleZone = new List<ScheduleZone>();

			Zones = new SortableObservableCollection<SelectationScheduleZoneViewModel>();
			var organisationResult = OrganisationHelper.GetSingle(organisation.UID);
			_doorUIDs = organisationResult != null ? organisationResult.DoorUIDs : new List<Guid>();

			var gkDoors = GKManager.Doors.Where(x => _doorUIDs.Any(y => y == x.UID));
			foreach (var door in gkDoors)
			{
				if (door.EnterZoneUID != Guid.Empty)
				{
					var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
					if (enterZone != null && !Zones.Any(x => x.ZoneUID == enterZone.UID))
						Zones.Add(new SelectationScheduleZoneViewModel(enterZone, schedule, door.UID));
				}

				if (door.ExitZoneUID != Guid.Empty)
				{
					var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
					if (exitZone != null && !Zones.Any(x => x.ZoneUID == exitZone.UID))
						Zones.Add(new SelectationScheduleZoneViewModel(exitZone, schedule, door.UID));
				}
			}

			Zones = new ObservableCollection<SelectationScheduleZoneViewModel>(Zones.OrderBy(x => x.No)); //TODO: 
			SelectedZone = Zones.FirstOrDefault();
		}

		public ObservableCollection<SelectationScheduleZoneViewModel> Zones { get; private set; }

		SelectationScheduleZoneViewModel _selectedZone;
		public SelectationScheduleZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			foreach (var zone in Zones)
			{
				if (zone.IsChecked)
				{
					ScheduleZone.Add(new ScheduleZone() { ZoneUID = zone.ZoneUID, DoorUID = zone.DoorUID , ScheduleUID = zone.ScheduleUID});
				}
			}
			return true;
		}
	}

	public class SelectationScheduleZoneViewModel : BaseViewModel
	{
		public Guid DoorUID { get; private set; }
		public Guid ZoneUID { get; private set; }
		public string Name { get; private set; }
		public int No { get; private set; }
		public string Description { get; private set; }
		public bool IsChecked { get; set; }
		public Guid ScheduleUID { get; set; }

		public SelectationScheduleZoneViewModel(GKSKDZone zone, Schedule schedule, Guid doorUID)
		{
			DoorUID = doorUID;
			ZoneUID = zone.UID;
			Name = zone.Name;
			No = zone.No;
			ScheduleUID = schedule.UID;
			Description = zone.Description;
			IsChecked = schedule.Zones.Any(x => x.ZoneUID == zone.UID);
		}
	}
}