using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		Schedule _schedule;
		public ScheduleZone ScheduleZone { get; private set; }
		 
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


			var doors = FiresecAPI.SKD.SKDManager.Doors.Where(x => organisation.DoorUIDs.Any(y => y == x.UID));
			Zones = new ObservableCollection<SelectationScheduleZoneViewModel>(); 
			foreach (var door in doors)
			{
				if (door != null && door.OutDevice != null && door.OutDevice.Zone != null && !Zones.Any(x => x.Zone.UID == door.OutDevice.Zone.UID))
					Zones.Add(new SelectationScheduleZoneViewModel(door.OutDevice.Zone, door.UID));
				if (door != null && door.InDevice != null && door.InDevice.Zone != null && !Zones.Any(x => x.Zone.UID == door.InDevice.Zone.UID))
					Zones.Add(new SelectationScheduleZoneViewModel(door.InDevice.Zone, door.UID));
			}
			SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == ScheduleZone.ZoneUID);
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
				if (_schedule.Zones.Any(x => x.ZoneUID == SelectedZone.Zone.UID && ScheduleZone.UID != x.UID))
				{
					MessageBoxService.ShowWarning("Выбранная зона уже включена");
					return false;
				}
			}
			ScheduleZone.ZoneUID = SelectedZone.Zone.UID;
			ScheduleZone.DoorUID = SelectedZone.DoorUID;
			return true;
		}
	}

	public class SelectationScheduleZoneViewModel:BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public Guid DoorUID { get; private set; }

		public SelectationScheduleZoneViewModel(SKDZone zone, Guid doorUID)
		{
			Zone = zone;
			DoorUID = doorUID;
		}
	}
}