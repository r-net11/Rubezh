using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	[SaveSizeAttribute]
	public class ZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		bool CanSelectRoot;

		public ZoneSelectationViewModel(Guid zoneUID, bool canSelectRoot)
		{
			Title = "Выбор зоны";
			CanSelectRoot = canSelectRoot;
			BuildTree();
			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}

			Select(zoneUID);
		}

		#region ZoneSelection
		public List<ZoneViewModel> AllZones;

		public void FillAllZones()
		{
			AllZones = new List<ZoneViewModel>();
			AddChildPlainZones(RootZone);
		}

		void AddChildPlainZones(ZoneViewModel parentViewModel)
		{
			AllZones.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainZones(childViewModel);
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				var deviceViewModel = AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedZone = deviceViewModel;
			}
		}
		#endregion

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

		ZoneViewModel _rootZone;
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

		void BuildTree()
		{
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			FillAllZones();
		}

		ZoneViewModel AddZoneInternal(SKDZone zone, ZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new ZoneViewModel(zone);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
				AddZoneInternal(childZone, zoneViewModel);
			return zoneViewModel;
		}

		protected override bool CanSave()
		{
			if (!CanSelectRoot && SelectedZone.Zone.IsRootZone)
				return false;
			return true;
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}