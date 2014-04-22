using System;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		private Schedule _schedule;
		public ScheduleZone ScheduleZone { get; private set; }

		public ScheduleZoneDetailsViewModel(Schedule schedule, ScheduleZone zone = null)
		{
			_schedule = schedule;
			if (zone == null)
			{
				Title = "Выбор помещения";
				zone = new ScheduleZone()
				{
					ScheduleUID = schedule.UID,
				};
			}
			else
				Title = "Редактирование помещения";
			ScheduleZone = zone;
			IsControl = zone.IsControl;

			AllZones = new List<ZoneViewModel>();
			RootZone = AddZoneInternal(FiresecAPI.SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = AllZones.FirstOrDefault(x => x.Zone.UID == ScheduleZone.ZoneUID);

			foreach (var z in AllZones)
			{
				z.ExpandToThis();
			}
		}

		public List<ZoneViewModel> AllZones;

		private ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedZone");
			}
		}

		private ZoneViewModel _rootZone;
		public ZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public ZoneViewModel[] RootZones
		{
			get { return new ZoneViewModel[] { RootZone }; }
		}

		private ZoneViewModel AddZoneInternal(FiresecAPI.SKDZone zone, ZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new ZoneViewModel(zone);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}

		private bool _isControl;
		public bool IsControl
		{
			get { return _isControl; }
			set
			{
				_isControl = value;
				OnPropertyChanged("IsControl");
			}
		}

		protected override bool CanSave()
		{
			return SelectedZone!= null && !SelectedZone.Zone.IsRootZone;
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
			ScheduleZone.IsControl = IsControl;
			return true;
		}
	}
}