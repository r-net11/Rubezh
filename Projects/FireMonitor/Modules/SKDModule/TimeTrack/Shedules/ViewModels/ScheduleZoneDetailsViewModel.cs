using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		Schedule _schedule;
		public ScheduleZone ScheduleZone { get; private set; }
		List<Guid> _doorUIDs;

		public ScheduleZoneDetailsViewModel(Schedule schedule, Organisation organisation, ScheduleZone sheduleZone = null)
		{
			_schedule = schedule;
			if (sheduleZone == null)
			{
				Title = "Выбор помещения";
				sheduleZone = new ScheduleZone()
				{
					ScheduleUID = schedule.UID,
				};
			}
			else
				Title = "Редактирование помещения";
			ScheduleZone = sheduleZone;

			Zones = new SortableObservableCollection<SelectationScheduleZoneViewModel>();
			var organisationResult = OrganisationHelper.GetSingle(organisation.UID);
			_doorUIDs = organisationResult != null ? organisationResult.DoorUIDs : new List<Guid>();

			var gkDoors = FiresecClient.GKManager.Doors.Where(x => _doorUIDs.Any(y => y == x.UID));
			foreach (var door in gkDoors)
			{
				if (door.EnterZoneUID != Guid.Empty)
				{
					var enterZone = FiresecClient.GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
					if (enterZone != null && !Zones.Any(x => x.ZoneUID == enterZone.UID))
						Zones.Add(new SelectationScheduleZoneViewModel(enterZone, door.UID));
				}

				if (door.ExitZoneUID != Guid.Empty)
				{
					var exitZone = FiresecClient.GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
					if (exitZone != null && !Zones.Any(x => x.ZoneUID == exitZone.UID))
						Zones.Add(new SelectationScheduleZoneViewModel(exitZone, door.UID));
				}
			}

			Zones = new ObservableCollection<SelectationScheduleZoneViewModel>(Zones.OrderBy(x => x.No)); //TODO: 
			SelectedZone = Zones.FirstOrDefault(x => x.ZoneUID == ScheduleZone.ZoneUID);
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
			if (SelectedZone != null)
			{
				if (_schedule.Zones.Any(x => x.ZoneUID == SelectedZone.ZoneUID && ScheduleZone.UID != x.UID))
				{
					MessageBoxService.ShowWarning("Выбранная зона уже включена");
					return false;
				}
			}
			ScheduleZone.ZoneUID = SelectedZone.ZoneUID;
			ScheduleZone.DoorUID = SelectedZone.DoorUID;
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

		public SelectationScheduleZoneViewModel(SKDZone zone, Guid doorUID)
		{
			DoorUID = doorUID;
			ZoneUID = zone.UID;
		//	Name = zone.PresentationName;
			Name = zone.Name;
			No = zone.No;
			Description = zone.Description;
		}

		public SelectationScheduleZoneViewModel(GKSKDZone zone, Guid doorUID)
		{
			DoorUID = doorUID;
			ZoneUID = zone.UID;
		//	Name = zone.PresentationName;
			Name = zone.Name;
			No = zone.No;
			Description = zone.Description;
		}
	}
}