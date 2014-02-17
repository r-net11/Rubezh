using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class ShedulePartDetailsViewModel : SaveCancelDialogViewModel
	{
		SheduleViewModel SheduleViewModel;
		bool IsNew;
		public EmployeeShedulePart ShedulePart { get; private set; }

		public ShedulePartDetailsViewModel(SheduleViewModel sheduleViewModel, EmployeeShedulePart shedulePart = null)
		{
			SheduleViewModel = sheduleViewModel;
			if (shedulePart == null)
			{
				Title = "Выбор помещения";
				IsNew = true;
				shedulePart = new EmployeeShedulePart();
			}
			else
			{
				Title = "Редактирование помещения";
				IsNew = false;
			}
			ShedulePart = shedulePart;
			IsControl = shedulePart.IsControl;

			AllZones = new List<SheduleZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = AllZones.FirstOrDefault(x => x.Zone.UID == ShedulePart.ZoneUID);

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
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
				if (SheduleViewModel.SheduleParts.Any(x => x.ShedulePart.ZoneUID == SelectedZone.Zone.UID && ShedulePart.UID != x.ShedulePart.UID))
				{
					MessageBoxService.ShowWarning("Выбранная зона уже включена");
					return false;
				}
			}
			ShedulePart.ZoneUID = SelectedZone.Zone.UID;
			ShedulePart.IsControl = IsControl;
			return true;
		}
	}
}