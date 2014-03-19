using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessZonesSelectationViewModel : BaseViewModel
	{
		Organization Organization;
		public List<CardZone> CardZones { get; private set; }
		Guid? ParentUID;
		HashSet<SKDZone> AllParentZones;

		public AccessZonesSelectationViewModel(Organization organization, List<CardZone> cardZones, Guid? parentUID)
		{
			Organization = organization;
			CardZones = cardZones;

			InitializeAllParentZones();
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = RootZone;
			ParentUID = parentUID;
			
			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}
		}

		void InitializeAllParentZones()
		{
			AllParentZones = new HashSet<SKDZone>();
			foreach (var zoneUID in Organization.ZoneUIDs)
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
					AddAllParents(zone);
				}
			}
		}
		void AddAllParents(SKDZone zone)
		{
			AllParentZones.Add(zone);
			if (zone.Parent != null)
				AddAllParents(zone.Parent);
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
			var zoneViewModel = new AccessZoneViewModel(zone, CardZones, x => { SelectedZone = x; });
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				if (AllParentZones.Any(x => x.UID == childZone.UID))
					AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
		#endregion

		public List<CardZone> GetCardZones()
		{
			CardZones = new List<CardZone>();
			foreach (var zone in AllZones)
			{
				if (zone.IsChecked)
				{
					var cardZone = new CardZone()
					{
						ZoneUID = zone.Zone.UID,
						IsAntiPassback = zone.IsAntiPassback,
						IsComission = zone.IsComission,
						IntervalType = zone.SelectedTimeCreteria.IntervalType,
						IntervalUID = zone.SelectedTimeType.UID,
						ParentUID = ParentUID
					};
					CardZones.Add(cardZone);
				}
			}
			return CardZones;
		}
	}
}