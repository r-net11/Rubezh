using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class AccessZonesSelectationViewModel : SaveCancelDialogViewModel
	{
		SKDCard Card;
		public AccessZonesSelectationViewModel(SKDCard card)
		{
			Card = card;
			Title = "Выбор зон";
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = RootZone;

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}
		}

		#region Zones
		public List<AccessZoneViewModel> AllZones;

		AccessZoneViewModel _selectedZone;
		public AccessZoneViewModel SelectedZone
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

		AccessZoneViewModel _rootZone;
		public AccessZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public AccessZoneViewModel[] RootZones
		{
			get { return new AccessZoneViewModel[] { RootZone }; }
		}

		AccessZoneViewModel AddZoneInternal(SKDZone zone, AccessZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new AccessZoneViewModel(zone);
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

		protected override bool Save()
		{
			Card.ZoneLinkUids = new List<Guid>();
			foreach (var zone in AllZones)
			{
				if (zone.IsChecked)
				{
					Card.ZoneLinkUids.Add(zone.Zone.UID);
				}
			}
			return true;
		}
	}
}