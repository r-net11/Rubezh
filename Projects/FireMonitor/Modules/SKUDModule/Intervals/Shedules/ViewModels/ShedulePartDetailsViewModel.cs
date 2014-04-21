using System;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class ShedulePartDetailsViewModel : SaveCancelDialogViewModel
	{
		SheduleViewModel SheduleViewModel;
		bool IsNew;
		public ScheduleZone Zone { get; private set; }

		public ShedulePartDetailsViewModel(SheduleViewModel sheduleViewModel, ScheduleZone zone = null)
		{
			SheduleViewModel = sheduleViewModel;
			if (zone == null)
			{
				Title = "Выбор помещения";
				IsNew = true;
				zone = new ScheduleZone();
			}
			else
			{
				Title = "Редактирование помещения";
				IsNew = false;
			}
			Zone = zone;
			IsControl = zone.IsControl;

			AllZones = new List<SheduleZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = AllZones.FirstOrDefault(x => x.Zone.UID == Zone.ZoneUID);

			foreach (var z in AllZones)
			{
				z.ExpandToThis();
			}
		}

		#region Zones
		public List<SheduleZoneViewModel> AllZones;

		SheduleZoneViewModel _selectedZone;
		public SheduleZoneViewModel SelectedZone
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

		SheduleZoneViewModel _rootZone;
		public SheduleZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public SheduleZoneViewModel[] RootZones
		{
			get { return new SheduleZoneViewModel[] { RootZone }; }
		}

		SheduleZoneViewModel AddZoneInternal(SKDZone zone, SheduleZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new SheduleZoneViewModel(zone);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
		#endregion

		bool _isControl;
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
				if (SheduleViewModel.SheduleParts.Any(x => x.ShedulePart.ZoneUID == SelectedZone.Zone.UID && Zone.UID != x.ShedulePart.UID))
				{
					MessageBoxService.ShowWarning("Выбранная зона уже включена");
					return false;
				}
			}
			Zone.ZoneUID = SelectedZone.Zone.UID;
			Zone.IsControl = IsControl;
			return true;
		}
	}
}